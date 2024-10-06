using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace dvmusrp
{
    public class Config
    {
        public DvmConfig dvm = new DvmConfig();
        public UsrpConfig usrp = new UsrpConfig();

        public Config() { /* stub */ }

        public static Config Load(string filePath)
        {
            var yamlContent = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
            return deserializer.Deserialize<Config>(yamlContent);
        }
    }

    public class DvmConfig
    {
        public string ReceiveAddress { get; set; }
        public string SendAddress { get; set; }

        public int ReceivePort { get; set; }
        public int SendPort { get; set; }

        public DvmConfig() { /* sub */ }
    }

    public class UsrpConfig
    {
        public string ReceiveAddress { get; set; }
        public string SendAddress { get; set; }

        public int ReceivePort { get; set; }
        public int SendPort { get; set; }

        public UsrpConfig() { /* sub */ }
    }
}
