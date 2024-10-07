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

namespace dvmusrp
{
    /// <summary>
    /// USRP networking class
    /// </summary>
    public class NetworkUSRP : Network
    {
        private NetworkDVM dvmNetwork;

        /// <summary>
        /// Creates an instance of <see cref="NetworkDVM"/>
        /// </summary>
        /// <param name="receivePort"></param>
        /// <param name="sendPort"></param>
        /// <param name="receiveAddress"></param>
        /// <param name="sendAddress"></param>
        public NetworkUSRP(int receivePort, int sendPort, string receiveAddress, string sendAddress) : base(receivePort, sendPort, receiveAddress, sendAddress) { }

        /// <summary>
        /// Helper to set the <see cref="NetworkDVM"/> instance
        /// </summary>
        /// <param name="dvmNetwork"></param>
        public void SetDVMNetwork(NetworkDVM dvmNetwork)
        {
            this.dvmNetwork = dvmNetwork;
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
                            var receivedResult = await udpClient.ReceiveAsync();

                            ForwardToDVM(receivedResult.Buffer);
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
        /// Forward, parse, and convert USRP to DVM
        /// </summary>
        /// <param name="packet"></param>
        private void ForwardToDVM(byte[] packet)
        {
            // TODO: Parse USRP header for possible DVM use
            byte[] audio = new byte[324]; // PCM length + PCM data
            byte[] usrpHeader = new byte[32];

            Array.Copy(packet, usrpHeader, usrpHeader.Length);

            if (packet.Length != 352)
                return;

            Array.Copy(packet, 32, audio, 4, audio.Length - 4);

            if (audio.Length != 324)
            {
                Console.WriteLine(audio.Length);
            }

            Utils.WriteBytes(320, ref audio, 0);

            // Console.WriteLine(Utils.HexDump(audio));

            dvmNetwork.SendData(audio);
        }

        /// <summary>
        /// Helper to send data to USRP
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            Send(data, sendAddress, sendPort);
        }
    }
}
