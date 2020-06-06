using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    class CameraPTC140Handler : IClientCallback
    {
        private SocketAutoConnectParser m_SocketClient;
        private CameraConnexionModel m_Connexion;

        public CameraPTC140Handler(CameraConnexionModel model)
        {
            m_Connexion = model;
        }

        public void Initialize()
        {
            if (m_SocketClient == null || !m_SocketClient.Connected)
            {
                m_SocketClient = new SocketAutoConnectParser();
                m_SocketClient.Initialize(m_Connexion.CameraName, m_Connexion.CameraHost, m_Connexion.CameraPort);
                m_SocketClient.OpenChanel(this);
            }
        }

        public void PanTiltUp()
        {
            if (m_SocketClient != null && m_SocketClient.Connected)
                m_SocketClient.SendData(new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, 0x17, 0x17, 0x03, 0x01, 0xFF });
        }

        public void PanTiltDown()
        {

        }

        public void PanTiltLeft()
        {

        }

        public void PanTiltRight()
        {

        }

        public void PanTiltUpLeft()
        {

        }

        public void PanTiltUpRight()
        {

        }

        public void PanTiltDownLeft()
        {

        }

        public void PanTiltDownRight()
        {

        }

        public void PanTiltStop()
        {

        }

        public void PanTiltHome()
        {

        }
        public void CameraMemoryReset()
        {

        }

        public void CameraMemorySet()
        {

        }

        public void CameraMemoryRecall()
        {

        }
        public void CompletionMessage()
        {
            App.logger.Debug("message OK");
        }
    }
}
