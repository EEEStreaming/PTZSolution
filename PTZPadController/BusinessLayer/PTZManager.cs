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
        private CameraConnexionModel m_ConnexionCamera1;
        private CameraPTC140Handler m_Camera1;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PTZManager class.
        /// </summary>
        public PTZManager()
        {
            m_ConnexionCamera1 = new CameraConnexionModel { CameraName = "CAM 1", CameraHost = "127.0.0.1", CameraPort = 5002 };
            m_Camera1 = new CameraPTC140Handler(m_ConnexionCamera1);
            m_Camera1.Initialize();

        }
        #endregion

        public void CameraPanTiltUp()
        {
            m_Camera1.PanTiltUp();
        }
    }
}
