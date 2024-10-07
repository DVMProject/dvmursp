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

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace dvmusrp
{
    /// <summary>
    /// Configuration object
    /// </summary>
    public class Config
    {
        public DvmConfig dvm = new DvmConfig();
        public UsrpConfig usrp = new UsrpConfig();

        public Config() { /* stub */ }

        /// <summary>
        /// Helper to load YAML config file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Config Load(string filePath)
        {
            var yamlContent = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
            return deserializer.Deserialize<Config>(yamlContent);
        }
    }

    /// <summary>
    /// DVM Configuration object
    /// </summary>
    public class DvmConfig
    {
        public string ReceiveAddress { get; set; }
        public string SendAddress { get; set; }

        public int ReceivePort { get; set; }
        public int SendPort { get; set; }

        public DvmConfig() { /* sub */ }
    }

    /// <summary>
    /// USRP Configuration object
    /// </summary>
    public class UsrpConfig
    {
        public string ReceiveAddress { get; set; }
        public string SendAddress { get; set; }

        public int ReceivePort { get; set; }
        public int SendPort { get; set; }

        public UsrpConfig() { /* sub */ }
    }
}
