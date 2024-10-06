using System.Net.Sockets;
using System.Text;

namespace dvmusrp
{
    public class NetworkDVM : Network
    {
        private NetworkUSRP usrpNetwork;

        public NetworkDVM(int receivePort, int sendPort, string receiveAddress, string sendAddress) : base(receivePort, sendPort, receiveAddress, sendAddress) { }

        public void SetUSRPNetwork(NetworkUSRP usrpNetwork)
        {
            this.usrpNetwork = usrpNetwork;
        }

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

                            ForwardToUSRP(receivedResult.Buffer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            });
        }

        private void ForwardToUSRP(byte[] packet)
        {
            byte[] usrpData = new byte[352];
            byte[] audio = new byte[320];
            byte[] usrpHeader = new byte[32];

            if (packet.Length != 324)
            {
                Console.WriteLine("DVM meta data is not yet supported");
                return;
            }

            Array.Copy(Encoding.ASCII.GetBytes("USRP"), usrpHeader, 4);
            Array.Copy(usrpHeader, usrpData, usrpHeader.Length);
            Array.Copy(packet, 4, audio, 0, audio.Length);
            Array.Copy(audio, 0, usrpData, 32, audio.Length);

            // Console.WriteLine(Utils.HexDump(usrpData));

            usrpNetwork.SendData(usrpData);
        }

        public void SendData(byte[] data)
        {
            Send(data, sendAddress, sendPort);
        }
    }
}
