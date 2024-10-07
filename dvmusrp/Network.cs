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
    /// Base networking class for UDP communications
    /// </summary>
    public abstract class Network
    {
        protected int receivePort;
        protected int sendPort;

        protected string receiveAddress;
        protected string sendAddress;

        protected UdpClient udpClient;

        /// <summary>
        /// Creatse an instance of <see cref="Network"/>
        /// </summary>
        /// <param name="receivePort"></param>
        /// <param name="sendPort"></param>
        /// <param name="receiveAddress"></param>
        /// <param name="sendAddress"></param>
        protected Network(int receivePort, int sendPort, string receiveAddress, string sendAddress)
        {
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.receiveAddress = receiveAddress;
            this.sendAddress = sendAddress;
        }

        /// <summary>
        /// Start the receive loop
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Helper to send data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destinationIp"></param>
        /// <param name="destinationPort"></param>
        protected void Send(byte[] data, string destinationIp, int destinationPort)
        {
            using (var client = new UdpClient())
            {
                client.Send(data, data.Length, destinationIp, destinationPort);
            }
        }
    }
}
