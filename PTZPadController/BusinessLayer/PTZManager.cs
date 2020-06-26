﻿using GalaSoft.MvvmLight.Messaging;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PTZPadController.BusinessLayer.AtemSwitcherHandler;

namespace PTZPadController.BusinessLayer
{
    public class PTZManager : IPTZManager
    {
        const short SPEED_MEDIUM = 15;

        private List<ICameraHandler> m_CameraList;
        private ISwitcherHandler m_AtemHandler;
        private bool m_UseTallyGreen;
        private bool m_Initialized;
        private ConfigurationModel m_Configuration;
        private bool m_IsStarted;

        public ICameraHandler CameraPreview { get; private set; }

        public ICameraHandler CameraProgram { get; private set; }

        public List<ICameraHandler> Cameras { get { return m_CameraList; } }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PTZManager class.
        /// </summary>
        public PTZManager()
        {
            m_CameraList = new List<ICameraHandler>();
            m_UseTallyGreen = false;
            m_Initialized = false;
            m_IsStarted = false;

        }
        #endregion

        #region Methods for the Initialization

        public void InitSeetings(ConfigurationModel cfg)
        {
            m_UseTallyGreen = cfg.UseTallyGreen;
            m_Initialized = true;
            m_Configuration = cfg;
        }

        public void AddCameraHandler(ICameraHandler camHandler)
        {
            m_CameraList.Add(camHandler);
        }

        public void SetSwitcherHandler(ISwitcherHandler atemHandler)
        {
            if (m_AtemHandler == null)
            {
                m_AtemHandler = atemHandler;
                Messenger.Default.Register<NotificationMessage<AtemSourceMessageArgs>>(this, AtemSourceChange);
            }
        }



        public void SendSwitcherTransition(TransitionEnum transition)
        {
            if (m_AtemHandler != null)
                m_AtemHandler.StartTransition(transition);
        }

        public void SetSwitcherPreview(string cameraName)
        {
            if (m_AtemHandler != null)
                m_AtemHandler.SetPreviewSource(cameraName);
        }

        private void AtemSourceChange(NotificationMessage<AtemSourceMessageArgs> msg)
        {
            if (m_AtemHandler != null)
            {
                if (msg.Notification == NotificationSource.ProgramSourceChanged)
                    AtemProgramSourceChange(msg.Sender, msg.Content);
                else if (msg.Notification == NotificationSource.PreviewSourceChanged)
                    AtemPreviewSourceChange(msg.Sender, msg.Content);
            }
        }
        #endregion

        public void UpdatePresetConfiguration(PresetEventArgs obj)
        {
            //CameraPreview = m_CameraList[0];//to test
            if (CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    var prestcfg = camcfg.PresetIcons.FirstOrDefault(x => x.PresetId == obj.PresetId);
                    if (prestcfg != null)
                    {
                        prestcfg.IconKey = obj.PresetIcon.Key;
                        ConfigurationFileParser.SaveConfiguration(m_Configuration, "Configuration.json");
                    }
                }
            }
        }

        public List<PresetIconSettingModel> GetPresetSettingFromPreview()
        {
            //CameraPreview = m_CameraList[0];//to test
            if (CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    return camcfg.PresetIcons;
                }
            }
            return null;
        }

        private void AtemPreviewSourceChange(object sender, AtemSourceMessageArgs e)
        {
            CameraMessageArgs args;
            if (CameraPreview != null)
            {
                var lRed = CameraPreview == CameraProgram;
                CameraPreview.Tally(lRed, false);

                args = new CameraMessageArgs { CameraName = CameraPreview.CameraName };
                if (lRed)
                    args.Status = CameraStatusEnum.Program;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));
            }
            CameraPreview = null;
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraPreview = cam;
                    CameraPreview.Tally(false, m_UseTallyGreen);

                    args = new CameraMessageArgs { CameraName = CameraPreview.CameraName, Status = CameraStatusEnum.Preview };
                    Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));

                }

            }

        }

        private void AtemProgramSourceChange(object sender, AtemSourceMessageArgs e)
        {
            CameraMessageArgs args;
            if (CameraProgram != null)
            {
                var lGreen = CameraPreview == CameraProgram;
                CameraProgram.Tally(false, m_UseTallyGreen ? lGreen : false);
                args = new CameraMessageArgs { CameraName = CameraProgram.CameraName };
                if (lGreen)
                    args.Status = CameraStatusEnum.Preview;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));

            }
            CameraProgram = null;

            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraProgram = cam;
                    CameraProgram.Tally(true, false);

                    if (CameraProgram.PanTileWorking)
                        CameraProgram.PanTiltStop();

                    if (CameraProgram.ZoomWorking)
                        CameraProgram.ZoomStop();

                    args = new CameraMessageArgs { CameraName = CameraProgram.CameraName, Status = CameraStatusEnum.Program };
                    Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));

                }

            }

        }

        private void UpdateTally()
        {
            throw new NotImplementedException();
        }

        public void StartUp()
        {
            List<Task> tasks = new List<Task>();
            Task t;
            //Connect cameras
            foreach (var cam in m_CameraList)
            {
                t = cam.ConnectTo().ContinueWith((t) =>
                {
                    if (cam.WaitForConnection())
                    {
                        cam.Tally(false, false);
                    }
                });
                tasks.Add(t);
            }

            //set camera 0 to preview to initialize UI.
            CameraPreview = m_CameraList[0];
            CameraMessageArgs args = new CameraMessageArgs
            {
                CameraName = m_CameraList[0].CameraName,
                Status = CameraStatusEnum.Preview
            };
            Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));

            //then Connect ATEM
            m_AtemHandler.ConnectTo();


            if (m_AtemHandler.WaitForConnection())
            {
                
                var programName = m_AtemHandler.GetCameraProgramName();
                var previewName = m_AtemHandler.GetCameraPreviewName();
                CameraProgram = m_CameraList.FirstOrDefault(x => x.CameraName == programName);
                if (CameraProgram != null)
                {
                    CameraProgram.Tally(true, false);
                    args = new CameraMessageArgs
                    {
                        CameraName = programName,
                        Status = CameraStatusEnum.Program
                    };
                    Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));
                }
                CameraPreview = m_CameraList.FirstOrDefault(x => x.CameraName == previewName);
                if (CameraPreview != null)
                {
                    CameraPreview.Tally(false, m_UseTallyGreen);
                    args = new CameraMessageArgs
                    {
                        CameraName = previewName,
                        Status = CameraStatusEnum.Preview
                    };
                    Messenger.Default.Send(new NotificationMessage<CameraMessageArgs>(args, NotificationSource.CameraStatusChanged));
                }
            }


            //Connect PAD


            if (Task.WaitAll(tasks.ToArray(), 2000))//increase delay for unit test in GitHUB CI machine. 1 sec is to short.
            {
                PTZLogger.Log.Info("System started, ready to use");
                CheckAtemAndCameraSetting();
            }
            else
                PTZLogger.Log.Info("System started, But some devices are not connected.");
            m_IsStarted = true;
        }

        private void CheckAtemAndCameraSetting()
        {
            //verify that every camera names, match with Atem camera names
            
            foreach (var camcfg in m_CameraList)
            {
               if (!m_AtemHandler.FindCameraName(camcfg.CameraName))
                  System.Windows.MessageBox.Show("Camera '"+camcfg.CameraName+"', not found on ATEM, please change your settings!", "PTZPad Error");

            }
        }

        public void CameraPanTiltUp()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltUp(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltUpLeft()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltUpLeft(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltUpRight()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltUpRight(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltDown()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltDown(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltDownLeft()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltDownLeft(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltDownRight()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltDownRight(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltLeft()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltLeft(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltRight()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltRight(SPEED_MEDIUM, SPEED_MEDIUM);
            }
        }

        void IPTZManager.CameraPanTiltStop()
        {
            if (CameraPreview != null)
            {
                CameraPreview.PanTiltStop();
            }
        }

        public void CameraZoomStop()
        {
            if (CameraPreview != null)
            {
                CameraPreview.ZoomStop();
            }
        }

        public void CameraZoomWide()
        {
            if (CameraPreview != null)
            {
                CameraPreview.ZoomWide();
            }
        }

        public void CameraZoomTele()
        {
            if (CameraPreview != null)
            {
                CameraPreview.ZoomTele();
            }
        }

        public void CameraCallPreset(int preset)
        {
            if (CameraPreview != null)
            {
                CameraPreview.CameraMemoryRecall((short)preset);
            }
        }

        public void CameraSetPreset(int preset)
        {
            if (CameraPreview != null)
                CameraPreview.CameraMemorySet((short)preset);
        }
    }
}