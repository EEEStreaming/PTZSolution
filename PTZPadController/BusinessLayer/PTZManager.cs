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

        public ICameraHandler CameraPreview { get; private set; }

        public ICameraHandler CameraProgram { get; private set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PTZManager class.
        /// </summary>
        public PTZManager()
        {
            m_CameraList = new List<ICameraHandler>();


        }
        #endregion

        #region Methods for the Initialization
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

        private void AtemPreviewSourceChange(object sender, SourceArgs e)
        {
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {
                    if (CameraPreview != null)
                        CameraPreview.Tally(false, false);

                    CameraPreview = cam;
                    CameraPreview.Tally(false, true);
                }

            }
        }

        private void AtemProgramSourceChange(object sender, SourceArgs e)
        {
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {
                    if (CameraProgram != null)
                        CameraProgram.Tally(false, false);

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
            //if (m_AtemHandler.waitForConnection())
           // {
           //     m_AtemHandler.setPreviewSource(Source.Source_1);
            //}
            //Connect PAD
        }
        #endregion

        public void CameraPanTiltUp()
        {
            if (m_CurrentCamera== null)
            {
                m_CurrentCamera = m_CameraList[0];
            }
            m_CurrentCamera.PanTiltUp();
        }
        public void CameraTally()
        {
            if (m_CurrentCamera == null)
            {
                m_CurrentCamera = m_CameraList[0];
            }

        }
        
    }
}