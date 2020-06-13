using Newtonsoft.Json;
using PTZPadController.DataAccessLayer;
using System;
using System.Text.Json;

namespace ConsolPTZPadTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new ConfigurationModel
            {
                AtemHost = "192.168.1.135",
                Cameras =
                {
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.131",
                        CameraName = "CAM 1",
                        CameraPort = 5002
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.132",
                        CameraName = "CAM 2",
                        CameraPort = 5002
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.133",
                        CameraName = "CAM 3",
                        CameraPort = 5002
                    }

                }
            };
            string jsonString;

            ConfigurationFileParser.SaveConfiguration(cfg, "Configuration.json");


            var cfg2 = ConfigurationFileParser.LoadConfigurationFile("Configuration.json");
            jsonString = JsonConvert.SerializeObject(cfg2, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(jsonString);
            Console.ReadLine();
        }
    }
}
