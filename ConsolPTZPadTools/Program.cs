using Newtonsoft.Json;
using PTZPadController.BusinessLayer;
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
                        CameraPort = 5002,
                        PresetIcons =
                        {
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "1"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "2"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "3"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "4"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "5"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "6"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "7"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "8"
                            }
                        }
                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.132",
                        CameraName = "CAM 2",
                        CameraPort = 5002,
                        PresetIcons =
                        {
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "1"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "2"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "3"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "4"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "5"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "6"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "7"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "8"
                            }
                        }

                    },
                    new CameraConnexionModel
                    {
                        CameraHost = "192.168.1.133",
                        CameraName = "CAM 3",
                        CameraPort = 5002,
                        PresetIcons =
                        {
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "1"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "2"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "3"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "4"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "5"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "6"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "7"
                            },
                            new PresetIconSettingModel
                            {
                                IconKey = "Man-02",
                                PresetId = "8"
                            }
                        }

                    }

                },
                GamePads =
                {
                    new HIDGamePadModel
                    {
                        HidDeviceName = "Thrustmaster T.16000M",
                        MappedCommands = new System.Collections.Generic.List<MappCommnadModel>
                        {
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPanTiltAxes, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.GenericDesktopX,HIDGamePadCommands.GenericDesktopY } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraZoomAxe,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.GenericDesktopZ } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset1,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button10 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset2,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button9 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset3,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button8 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset4,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button14 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset5,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button15 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset6,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button16 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera1SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button3 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera2SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button2 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera3SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button4 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.SwitcherCut, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button5 } },
                            new MappCommnadModel
                            {
                                PTZCommand = GamePadHandlerCommands.SwitcherMix,
                                GamePadCommand = new HIDGamePadCommands[] { HIDGamePadCommands.Button6 }
                            },
                            new MappCommnadModel
                            {
                                PTZCommand = GamePadHandlerCommands.CameraZoomAxe,
                                GamePadCommand = new HIDGamePadCommands[] { HIDGamePadCommands.GenericDesktopHatSwitch }
                            },

                        }
                    },
                    new HIDGamePadModel
                    {
                        HidDeviceName = "Logitech Logitech RumblePad 2 USB",
                        MappedCommands = new System.Collections.Generic.List<MappCommnadModel>
                        {
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPanTiltAxes, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.GenericDesktopX,HIDGamePadCommands.GenericDesktopY } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraZoomAxe,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.GenericDesktopRz, HIDGamePadCommands.GenericDesktopHatSwitch} },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset1,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button5 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset2,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button6 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset3,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button7 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset4,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button8 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset5,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button9 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraPreset6,  GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button10 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera1SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button1 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera2SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button2 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.Camera3SetPreview, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button3 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.SwitcherCut, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button4 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraFocusAutoOnePushSwitchMode, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button11 } },
                            new MappCommnadModel { PTZCommand = GamePadHandlerCommands.CameraFocusOnePushTriger, GamePadCommand = new HIDGamePadCommands[]{HIDGamePadCommands.Button12 } },
                        }
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
