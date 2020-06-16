using NSubstitute;
using NUnit.Framework;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTestPTZPadController.DataAccessLayer
{
    public class ConfigurationParserUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSaveAndLoadConfiguration()
        {
            ConfigurationModel cfg = new ConfigurationModel 
            { 
                AtemHost = "192.168.10.145", 
                UseTallyGreen = true, 
                Cameras = new List<CameraConnexionModel> 
                { 
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.20", CameraName = "CAM 1", CameraPort = 5002
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.21", CameraName = "CAM 2", CameraPort = 5002
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.22", CameraName = "CAM 3", CameraPort = 5002
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.23", CameraName = "CAM 4", CameraPort = 5002
                    }
                }
            };

            ConfigurationFileParser.SaveConfiguration(cfg, "test-configuration.json");

            Assert.IsTrue(File.Exists("test-configuration.json"));

            ConfigurationModel cfg2 = ConfigurationFileParser.LoadConfigurationFile("test-configuration.json");

            StringAssert.Contains(cfg.AtemHost, cfg2.AtemHost);
            StringAssert.Contains(cfg.AtemHost, cfg2.AtemHost);
            Assert.IsTrue(cfg.UseTallyGreen == cfg2.UseTallyGreen);
            Assert.IsTrue(cfg.Cameras.Count == cfg2.Cameras.Count);
            Assert.IsTrue(cfg.Cameras.Count == cfg2.Cameras.Count);
            for (int i = 0; i < cfg.Cameras.Count; i++)
            {
                Assert.IsTrue(cfg.Cameras[i].CameraHost == cfg2.Cameras[i].CameraHost, "camera host is different on index :{0}",i);
                Assert.IsTrue(cfg.Cameras[i].CameraName == cfg2.Cameras[i].CameraName, "camera name is different on index :{0}", i);
                Assert.IsTrue(cfg.Cameras[i].CameraPort == cfg2.Cameras[i].CameraPort, "camera port is different on index :{0}", i);
            }

        }
    }
}
