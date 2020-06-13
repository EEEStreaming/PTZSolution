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
    public class PTZManager : IPTZManager
    {
        private List<ICameraHandler> m_CameraList;
        private IAtemSwitcherHandler m_AtemHandler;
        private bool m_UseTallyGreen;

        public ICameraHandler CameraPreview { get; private set; }

        public ICameraHandler CameraProgram { get; private set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PTZManager class.
        /// </summary>
        public PTZManager()
        {
            m_CameraList = new List<ICameraHandler>();
            m_UseTallyGreen = false;


        }
        #endregion

        #region Methods for the Initialization

        public void InitSeetings(ConfigurationModel cfg)
        {
            m_UseTallyGreen = cfg.UseTallyGreen;
        }
       
        public void AddCcameraHandler(ICameraHandler camHandler)
        {
            m_CameraList.Add(camHandler);
        }

        public void SetAtemHandler(IAtemSwitcherHandler atemHandler)
        {
            m_AtemHandler = atemHandler;
            m_AtemHandler.ProgramSourceChanged += AtemProgramSourceChange;
            m_AtemHandler.PreviewSourceChanged += AtemPreviewSourceChange;
        }
        #endregion

        private void AtemPreviewSourceChange(object sender, SourceArgs e)
        {
            if (CameraPreview != null)
                CameraPreview.Tally(CameraPreview == CameraProgram, false);
            CameraPreview = null;
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraPreview = cam;
                    CameraPreview.Tally(false, m_UseTallyGreen);
                }

            }
            
        }

        private void AtemProgramSourceChange(object sender, SourceArgs e)
        {
            if (CameraProgram != null)
                CameraProgram.Tally(false, m_UseTallyGreen?CameraPreview == CameraProgram:false);

            CameraProgram = null;

            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraProgram = cam;
                    CameraProgram.Tally(true, false);

                    if (CameraProgram.PanTileWorking)
                        CameraProgram.StopPanTile();

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
                var prgName = m_AtemHandler.GetCameraProgramName();
                var previewName = m_AtemHandler.GetCameraPreviewName();
                CameraProgram = m_CameraList.FirstOrDefault(x => x.CameraName == prgName);
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

        }

        
    }
}