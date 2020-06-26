using GalaSoft.MvvmLight.Ioc;
using NUnit.Framework;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPTZPadController.BusinessLayer
{
    public class PTZManagerATEMandConfigUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCheckAtemAndCameraSetting()
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

            SimpleIoc.Default.Register<IDisplayMessage, AssertDisplayMessage>();


            PTZManager manager = new PTZManager();
            manager.InitSeetings(cfg);
            MockAtemMissmatchHandler atem = new MockAtemMissmatchHandler();
            atem.Configuration = cfg;
            //Create
            List<MockCameraMissmatchHandler> cams = new List<MockCameraMissmatchHandler>(cfg.Cameras.Count);
            for (int i = 0; i < cfg.Cameras.Count; i++)
            {
                cams.Add(new MockCameraMissmatchHandler { Configuration = cfg.Cameras[i] });
                manager.AddCameraHandler(cams[i]);
            }


            //initialize Program and Preview
            manager.SetSwitcherHandler(atem);


            //startup
            manager.StartUp();

            //after startup Cam preview 0 is green, program 1 red and other off
            Assert.IsTrue(AssertDisplayMessage.ShowError,"Message box wasnot called!");

            SimpleIoc.Default.Unregister<IDisplayMessage>();
        }
    }

    class AssertDisplayMessage : IDisplayMessage
    {
        public static bool ShowError = false;
        public void Show(string message)
        {
            Assert.IsTrue(message.StartsWith("Configruation doesn't match with ATEM settings."), "It's not the right message");
            ShowError = true;
        }
    }

    class MockAtemMissmatchHandler : ISwitcherHandler
    {
        private bool bConnected = false;

        int iCamPreview;
        int iCamProgram;
        public ConfigurationModel Configuration { get; internal set; }

        //public bool IsConnected { get { return bConnected; } }

        public event EventHandler<AtemSourceMessageArgs> PreviewSourceChanged;
        public event EventHandler<AtemSourceMessageArgs> ProgramSourceChanged;

        public void ConnectTo()
        {
            bConnected = true;
            iCamPreview = 0;
            iCamProgram = 1;
        }


        public void Disconnect()
        {
            bConnected = false;
        }

        public bool FindCameraName(string cameraName)
        {
            return false;
        }

        public string GetCameraPreviewName()
        {
            return Configuration.Cameras[iCamPreview].CameraName;
        }

        public string GetCameraProgramName()
        {
            return Configuration.Cameras[iCamProgram].CameraName;
        }

        public void SetPreviewSource(string cameraName)
        {
            throw new NotImplementedException();
        }

        public void StartTransition(TransitionEnum transition)
        {
            throw new NotImplementedException();
        }

        public bool WaitForConnection()
        {
            return bConnected;
        }

        internal void CutInput()
        {
         }
    }


    class MockCameraMissmatchHandler : ICameraHandler
    {
        private Task tConnect;

        public string CameraName { get { return Configuration.CameraName; } }

        public bool PanTileWorking { get; internal set; }

        public bool ZoomWorking { get; internal set; }

        public CameraConnexionModel Configuration { get; internal set; }

        public MockTallyStatus TallyStatus { get; internal set; }

        public ICameraParserModel Parser { get; internal set; }

        public Task ConnectTo()
        {
            tConnect = Task.Run(() =>
            {
                PanTileWorking = false;
                ZoomWorking = false;
            });

            return tConnect;
        }
        public bool WaitForConnection()
        {
            //tConnect.Wait();
            return true;
        }
        public void Initialize(ICameraParser camParser)
        {
            PanTileWorking = false;
            ZoomWorking = false;
        }

        public void PanTiltDown(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltDownLeft(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltDownRight(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltLeft(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltRight(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltStop()
        {
            PanTileWorking = false;
        }

        public void PanTiltUp(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltUpLeft(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void PanTiltUpRight(short panSpeed, short tiltSpeed)
        {
            PanTileWorking = true;
        }

        public void Tally(bool ledRed, bool ledGreen)
        {
        }



        public void ZoomStop()
        {
            throw new NotImplementedException();
        }

        public void ZoomTele()
        {
            throw new NotImplementedException();
        }

        public void ZoomWide()
        {
            throw new NotImplementedException();
        }

        void ICameraHandler.CameraMemoryRecall(short memory)
        {
            throw new NotImplementedException();
        }

        void ICameraHandler.CameraMemoryReset(short memory)
        {
            throw new NotImplementedException();
        }

        void ICameraHandler.CameraMemorySet(short memory)
        {
            throw new NotImplementedException();
        }

        void ICameraHandler.PanTiltHome()
        {
            throw new NotImplementedException();
        }
    }
}

