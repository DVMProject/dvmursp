using System.Net.Sockets;
using System.Text;

namespace dvmusrp
{
    public class NetworkUSRP : Network
    {
        private NetworkDVM dvmNetwork;

        public NetworkUSRP(int receivePort, int sendPort, string receiveAddress, string sendAddress) : base(receivePort, sendPort, receiveAddress, sendAddress) { }

        public void SetDVMNetwork(NetworkDVM dvmNetwork)
        {
            this.dvmNetwork = dvmNetwork;
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

        private void ForwardToDVM(byte[] packet)
        {
            // TODO: Parse meta data for possible DVM use
            byte[] audio = new byte[324];
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

        public void SendData(byte[] data)
        {
            Send(data, sendAddress, sendPort);
        }
    }
}
