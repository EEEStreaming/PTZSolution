using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace PTZPadController.DataAccessLayer
{
    public class ConfigurationFileParser
    {
        public static ConfigurationModel LoadConfigurationFile(string fileName)
        {
            var jsonString = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<ConfigurationModel>(jsonString);
        }


        public static void SaveConfiguration(ConfigurationModel cfg, string fileName)
        {

        }
    }
}
