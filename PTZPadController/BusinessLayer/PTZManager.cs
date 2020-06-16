using GalaSoft.MvvmLight.Messaging;
using PTZPadController.Common;
using PTZPadController.DataAccessLayer;
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
    public class CameraEventArgs : EventArgs
    {
        public CameraStatusEnum Status { get; set; }
        public string CameraName { get; set; }
    }

    public class PTZManager : IPTZManager
    {
        const short SPEED_MEDIUM = 15;

        private List<ICameraHandler> m_CameraList;
        private IAtemSwitcherHandler m_AtemHandler;
        private bool m_UseTallyGreen;
        private bool m_Initialized;

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

        }
        #endregion

        #region Methods for the Initialization

        public void InitSeetings(ConfigurationModel cfg)
        {
            m_UseTallyGreen = cfg.UseTallyGreen;
            m_Initialized = true;
        }
       
        public void AddCameraHandler(ICameraHandler camHandler)
        {
            m_CameraList.Add(camHandler);
        }

        public void SetAtemHandler(IAtemSwitcherHandler atemHandler)
        {
            m_AtemHandler = atemHandler;
            Messenger.Default.Register<NotificationMessage<AtemSourceArgs>>(this, AtemSourceChange);
        }

        private void AtemSourceChange(NotificationMessage<AtemSourceArgs> msg)
        {
            if (msg.Notification == ConstMessages.ProgramSourceChanged)
                AtemProgramSourceChange(msg.Sender, msg.Content);
            else if (msg.Notification == ConstMessages.PreviewSourceChanged)
                AtemPreviewSourceChange(msg.Sender, msg.Content);
        }
        #endregion

        private void AtemPreviewSourceChange(object sender, AtemSourceArgs e)
        {
            CameraEventArgs args;
            if (CameraPreview != null)
            {
                var lRed = CameraPreview == CameraProgram;
                CameraPreview.Tally(lRed, false);

                args = new CameraEventArgs { CameraName = CameraPreview.CameraName };
                if (lRed)
                    args.Status = CameraStatusEnum.Program;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send<NotificationMessage<CameraEventArgs>>(new NotificationMessage<CameraEventArgs>(args, ConstMessages.CameraStatusChanged));
            }
            CameraPreview = null;
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraPreview = cam;
                    CameraPreview.Tally(false, m_UseTallyGreen);

                    args = new CameraEventArgs { CameraName = CameraPreview.CameraName, Status = CameraStatusEnum.Preview };
                    Messenger.Default.Send<NotificationMessage<CameraEventArgs>>(new NotificationMessage<CameraEventArgs>(args, ConstMessages.CameraStatusChanged));

                }

            }
            
        }

        private void AtemProgramSourceChange(object sender, AtemSourceArgs e)
        {
            CameraEventArgs args;
            if (CameraProgram != null)
            {
                var lGreen = CameraPreview == CameraProgram;
                CameraProgram.Tally(false, m_UseTallyGreen ? lGreen : false);
                args = new CameraEventArgs { CameraName = CameraProgram.CameraName };
                if (lGreen)
                    args.Status = CameraStatusEnum.Preview;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send<NotificationMessage<CameraEventArgs>>(new NotificationMessage<CameraEventArgs>(args, ConstMessages.CameraStatusChanged));

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

                    args = new CameraEventArgs { CameraName = CameraProgram.CameraName, Status = CameraStatusEnum.Program };
                    Messenger.Default.Send<NotificationMessage<CameraEventArgs>>(new NotificationMessage<CameraEventArgs>(args, ConstMessages.CameraStatusChanged));

                }

            }

        }

        private void UpdateTally()
        {
            throw new NotImplementedException();
        }

        public void StartUp()
        {
            //Connect cameras
            foreach (var cam in m_CameraList)
            {
                cam.Connect();
            }

            //Basic reset
            foreach (var cam in m_CameraList)
            {
                cam.Tally(false, false);
            }

            //Connect ATEM
            m_AtemHandler.connect();
            if (m_AtemHandler.waitForConnection() )
            {
                var programName = m_AtemHandler.GetCameraProgramName();
                var previewName = m_AtemHandler.GetCameraPreviewName();
                CameraProgram = m_CameraList.FirstOrDefault(x => x.CameraName == programName);
                if (CameraProgram != null)
                    CameraProgram.Tally(true, false);
                CameraPreview = m_CameraList.FirstOrDefault(x => x.CameraName == previewName);
                if (CameraPreview != null)
                    CameraPreview.Tally(false, m_UseTallyGreen);
            }
            //Connect PAD
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
    }
}