using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;

namespace PTZPadController.DataAccessLayer
{
    public class ConfigurationFileParser
    {
        public static ConfigurationModel LoadConfigurationFile(string fileName)
        {
            var jsonString = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<ConfigurationModel>(jsonString);
        }


        public static void SaveConfiguration(ConfigurationModel cfg, string fileName)
        {
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, cfg);
            }
        }
    }
}
