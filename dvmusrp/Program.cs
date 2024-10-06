using dvmusrp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var configFilePath = GetConfigFilePath(args);

        if (string.IsNullOrEmpty(configFilePath))
        {
            Console.WriteLine("Usage: dvmusrp -c configfile.yml");
            return;
        }

        var config = Config.Load(configFilePath);

        var usrpNetwork = new NetworkUSRP(config.usrp.ReceivePort, config.usrp.SendPort, config.usrp.ReceiveAddress, config.usrp.SendAddress);
        var dvmNetwork = new NetworkDVM(config.dvm.ReceivePort, config.dvm.SendPort, config.dvm.ReceiveAddress, config.dvm.ReceiveAddress);

        usrpNetwork.SetDVMNetwork(dvmNetwork);
        dvmNetwork.SetUSRPNetwork(usrpNetwork);

        usrpNetwork.Start();
        dvmNetwork.Start();

        await Task.Delay(Timeout.Infinite);
    }

    static string GetConfigFilePath(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-c" && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
