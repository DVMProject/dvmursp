// SPDX-License-Identifier: AGPL-3.0-only
/**
* Digital Voice Modem - DVMUSRP
* AGPLv3 Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
* @package DVM / DVMUSRP
* @license AGPLv3 License (https://opensource.org/licenses/AGPL-3.0)
*
*   Copyright (C) 2024 Caleb, KO4UYJ
*
*/

using System.Net.Sockets;
using System.Text;

namespace dvmusrp
{
    /// <summary>
    /// DVM networking class
    /// </summary>
    public class NetworkDVM : Network
    {
        private NetworkUSRP usrpNetwork;
        private DateTime lastPacketReceivedTime;
        private TimeSpan callTimeout = TimeSpan.FromSeconds(1);
        private bool eotSent = false;

        /// <summary>
        /// Creates an instance of <see cref="NetworkDVM"/>
        /// </summary>
        /// <param name="receivePort"></param>
        /// <param name="sendPort"></param>
        /// <param name="receiveAddress"></param>
        /// <param name="sendAddress"></param>
        public NetworkDVM(int receivePort, int sendPort, string receiveAddress, string sendAddress) : base(receivePort, sendPort, receiveAddress, sendAddress)
        {
            lastPacketReceivedTime = DateTime.Now;
        }

        /// <summary>
        /// Helper to set the instance of <see cref="NetworkUSRP"/>
        /// </summary>
        /// <param name="usrpNetwork"></param>
        public void SetUSRPNetwork(NetworkUSRP usrpNetwork)
        {
            this.usrpNetwork = usrpNetwork;
        }

        /// <summary>
        /// Start the receive loop
        /// </summary>
        public override void Start()
        {
            Task.Run(async () =>
            {
                using (udpClient = new UdpClient(receivePort))
                {
                    while (true)
                    {
                        try
                        {
                            Task<UdpReceiveResult> receivedTask = udpClient.ReceiveAsync();
                            Task timeoutTask = Task.Delay(callTimeout);

                            Task completedTask = await Task.WhenAny(receivedTask, timeoutTask);

                            if (completedTask == timeoutTask)
                            {
                                if (!eotSent)
                                {
                                    SendUsrpEOT();
                                    eotSent = true;
                                }
                                continue;
                            }

                            eotSent = false;

                            lastPacketReceivedTime = DateTime.Now;

                            ForwardToUSRP(receivedTask.Result.Buffer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Forward, parse, and convert DVM to USRP
        /// </summary>
        /// <param name="packet"></param>
        private void ForwardToUSRP(byte[] packet)
        {
            byte[] usrpData = new byte[352]; // 32 byte header + PCM data
            byte[] audio = new byte[320];
            byte[] usrpHeader = new byte[32];

            if (packet.Length != 324)
            {
                Console.WriteLine($"Unexpected DVM data length: {packet.Length}");
                return;
            }

            if (packet.Length == 332)
            {
                Array.Copy(packet, 12, audio, 0, audio.Length); // Skip DVM meta data
            } else
            {
                Array.Copy(packet, 4, audio, 0, audio.Length); // No meta data, just skip PCM length
            }

            usrpHeader[15] = 1; // Set PTT state to on

            Array.Copy(Encoding.ASCII.GetBytes("USRP"), usrpHeader, 4);
            Array.Copy(usrpHeader, usrpData, usrpHeader.Length);
            Array.Copy(audio, 0, usrpData, 32, audio.Length);

            // Console.WriteLine("USRP DATA:\n" + Utils.HexDump(usrpData));

            usrpNetwork.SendData(usrpData);
        }

        /// <summary>
        /// Helper to send a EOT to USRP
        /// </summary>
        public void SendUsrpEOT()
        {
            byte[] usrpHeader = new byte[32];
            Array.Copy(Encoding.ASCII.GetBytes("USRP"), usrpHeader, 4);

            usrpNetwork.SendData(usrpHeader);
        }

        /// <summary>
        /// Helper to send data to DVM
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            Send(data, sendAddress, sendPort);
        }
    }
}
