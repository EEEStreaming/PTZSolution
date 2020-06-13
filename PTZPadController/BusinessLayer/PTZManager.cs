using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    public class PTZManager : IPTZManager
    {
        private List<ICameraHandler> m_CameraList;
        private ICameraHandler m_CurrentCamera;
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
                cam.Tally(true, false);
            }

            //Connect ATEM

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
