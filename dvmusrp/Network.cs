using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace dvmusrp
{
    public abstract class Network
    {
        protected int receivePort;
        protected int sendPort;

        protected string receiveAddress;
        protected string sendAddress;

        protected UdpClient udpClient;

        protected Network(int receivePort, int sendPort, string receiveAddress, string sendAddress)
        {
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            this.receiveAddress = receiveAddress;
            this.sendAddress = sendAddress;
        }

        public abstract void Start();

        protected void Send(byte[] data, string destinationIp, int destinationPort)
        {
            using (var client = new UdpClient())
            {
                client.Send(data, data.Length, destinationIp, destinationPort);
            }
        }
    }
}
