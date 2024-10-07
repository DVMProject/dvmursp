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

using dvmusrp;

/// <summary>
/// 
/// </summary>
internal class Program
{
    /// <summary>
    /// Application entry point
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        var configFilePath = GetConfigFilePath(args);

        if (string.IsNullOrEmpty(configFilePath))
        {
            Console.WriteLine("Usage: dvmusrp -c config.yml");
            return;
        }

        Config config = Config.Load(configFilePath);

        NetworkUSRP usrpNetwork = new NetworkUSRP(config.usrp.ReceivePort, config.usrp.SendPort, config.usrp.ReceiveAddress, config.usrp.SendAddress);
        NetworkDVM dvmNetwork = new NetworkDVM(config.dvm.ReceivePort, config.dvm.SendPort, config.dvm.ReceiveAddress, config.dvm.ReceiveAddress);

        usrpNetwork.SetDVMNetwork(dvmNetwork);
        dvmNetwork.SetUSRPNetwork(usrpNetwork);

        usrpNetwork.Start();
        dvmNetwork.Start();

        Console.WriteLine("USRP: ");
        Console.WriteLine($"    Receive Port: {config.usrp.ReceivePort}");
        Console.WriteLine($"    Send Port: {config.usrp.SendPort}");
        Console.WriteLine($"    Receive Address: {config.usrp.ReceiveAddress}");
        Console.WriteLine($"    Send Address: {config.usrp.SendAddress}");
        Console.WriteLine("DVM: ");
        Console.WriteLine($"    Receive Port: {config.dvm.ReceivePort}");
        Console.WriteLine($"    Send Port: {config.dvm.SendPort}");
        Console.WriteLine($"    Receive Address: {config.dvm.ReceiveAddress}");
        Console.WriteLine($"    Send Address: {config.dvm.SendAddress}");
        Console.WriteLine("DVMUSRP Started");

        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Helper to psrse args
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
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
