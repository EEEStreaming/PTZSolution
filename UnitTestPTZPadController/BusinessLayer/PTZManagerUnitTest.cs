using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;

namespace UnitTestPTZPadController.BusinessLayer
{
    public class PTZManagerUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPTZManager()
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

            PTZManager manager = new PTZManager();
            manager.InitSeetings(cfg);
            MockAtemHandler atem = new MockAtemHandler();
            atem.Configuration = cfg;
            //Create
            List<MockCameraHandler> cams = new List<MockCameraHandler>(cfg.Cameras.Count);
            for (int i = 0; i < cfg.Cameras.Count; i++)
            {
                cams.Add(new MockCameraHandler { Configuration = cfg.Cameras[i] });
                manager.AddCameraHandler(cams[i]);
            }


            //initialize Program and Preview
            manager.SetSwitcherHandler(atem);


            //startup
            manager.StartUp();

            //after startup Cam preview 0 is green, program 1 red and other off
            Assert.IsTrue(cams[0].TallyStatus == MockTallyStatus.Green,"Tally camera is not green");
            Assert.IsTrue(cams[1].TallyStatus == MockTallyStatus.Red,"Tally camera is not red");
            Assert.IsTrue(cams[2].TallyStatus == MockTallyStatus.Off,"Tally camera is not off");
            Assert.IsTrue(cams[3].TallyStatus == MockTallyStatus.Off,"Tally camera is not off");


            Assert.AreEqual(manager.CameraPreview, cams[0], "Camera Preview not the set");
            Assert.AreEqual(manager.CameraProgram, cams[1], "Camera Program not the set");


            atem.CutInput();
            Assert.IsTrue(cams[1].TallyStatus == MockTallyStatus.Green, "Tally camera is not green");
            Assert.IsTrue(cams[0].TallyStatus == MockTallyStatus.Red, "Tally camera is not red");
            Assert.IsTrue(cams[2].TallyStatus == MockTallyStatus.Off, "Tally camera is not off");
            Assert.IsTrue(cams[3].TallyStatus == MockTallyStatus.Off, "Tally camera is not off");

            atem.CutInput();
            Assert.IsTrue(cams[0].TallyStatus == MockTallyStatus.Green, "Tally camera is not green");
            Assert.IsTrue(cams[1].TallyStatus == MockTallyStatus.Red, "Tally camera is not red");
            Assert.IsTrue(cams[2].TallyStatus == MockTallyStatus.Off, "Tally camera is not off");
            Assert.IsTrue(cams[3].TallyStatus == MockTallyStatus.Off, "Tally camera is not off");

        }
    }

    class MockAtemHandler : ISwitcherHandler
    {
        private bool bConnected = false;

        int iCamPreview;
        int iCamProgram;
        public ConfigurationModel Configuration { get; internal set; }

        //public bool IsConnected { get { return bConnected; } }

        public event EventHandler<AtemSourceMessageArgs> PreviewSourceChanged;
        public event EventHandler<AtemSourceMessageArgs> ProgramSourceChanged;

        public Task ConnectTo()
        {
            return Task.Run(() =>
            {
                bConnected = true;
                iCamPreview = 0;
                iCamProgram = 1;
            });
        }


        public void Disconnect()
        {
            bConnected = false;
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
            var i = iCamPreview;
            iCamPreview = iCamProgram;
            iCamProgram = i;


            AtemSourceMessageArgs args = new AtemSourceMessageArgs();
            args.PreviousInputName = Configuration.Cameras[iCamProgram].CameraName;
            args.CurrentInputName = Configuration.Cameras[iCamPreview].CameraName;
            Messenger.Default.Send<NotificationMessage<AtemSourceMessageArgs>>(new NotificationMessage<AtemSourceMessageArgs>(args, NotificationSource.PreviewSourceChanged));

            
            args.PreviousInputName = Configuration.Cameras[iCamPreview].CameraName;
            args.CurrentInputName = Configuration.Cameras[iCamProgram].CameraName;
            Messenger.Default.Send<NotificationMessage<AtemSourceMessageArgs>>(new NotificationMessage<AtemSourceMessageArgs>(args, NotificationSource.ProgramSourceChanged));
        }

        void ISwitcherHandler.ConnectTo()
        {
            throw new NotImplementedException();
        }
    }

    enum MockTallyStatus
    {
        Red,
        Green,
        Off
    }

    class MockCameraHandler : ICameraHandler
    {

        
        public string CameraName  { get { return Configuration.CameraName; } }

        public bool PanTileWorking { get; internal set; }

        public bool ZoomWorking { get; internal set; }

        public CameraConnexionModel Configuration { get; internal set; }

        public MockTallyStatus TallyStatus{ get; internal set; }

        public ICameraParserModel Parser { get; internal set; }

        public Task ConnectTo()
        {
            return Task.Run(() =>
            {
                PanTileWorking = false;
                ZoomWorking = false;
            });
        }
        public bool WaitForConnection()
        {
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
            if (ledRed && ledGreen)
                throw new ArgumentException("Led Red and Green are ON!");

            if (ledRed)
                TallyStatus = MockTallyStatus.Red;
            else if (ledGreen)
                TallyStatus = MockTallyStatus.Green;
            else
                TallyStatus = MockTallyStatus.Off;
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

