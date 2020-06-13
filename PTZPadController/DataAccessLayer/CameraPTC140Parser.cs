using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.DataAccessLayer
{
    public class CameraPTC140Parser : ICameraParser, IClientCallback
    {
        private ISocketParser m_SocketClient;

        public bool Connected { get { return (m_SocketClient != null) && (m_SocketClient.Connected); } }

        public void CompletionMessage(string message)
        {
            PTZLogger.Log.Info("Message reçu {0}", message);
            if (message == "00-08-90-41-FF-90-51-FF")
            {
                //TODO
            }

            //TODO
        }

        public void Initialize(ISocketParser socket)
        {
            if (m_SocketClient == null || !m_SocketClient.Connected)
            {
                m_SocketClient = socket;
            }
        }

        public void Connect()
        {
            m_SocketClient.Connect();
        }

        public void Disconnect()
        {
            m_SocketClient.Disconnect();
        }

        /// <summary>
        /// Led control 
        /// </summary>
        /// <param name="ledRed"></param>
        /// <param name="ledGreen"></param>
        public void Tally(bool ledRed, bool ledGreen)
        {
            //If both true, turn on ledRed
            byte red = (byte) (ledRed ? 0x02 : 0x03);
            byte green = (byte)(ledGreen && !ledRed ? 0x02 : 0x03);

            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                m_SocketClient.SendData(new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x7E, 0x01, 0x0A, 0x00, red, green, 0xFF });
            }
        }

        private byte ConvertSpeed(short speed)
        {
            byte byteSpeed = (byte) (speed > 255 ? 255 : speed < 0 ? 0: speed);
            return byteSpeed;
        }

        public void PanTiltUp(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltUp({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x03, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDown(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltDown({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x03, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltLeft(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltLeft({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x01, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltRight(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltRight({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x02, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltUpLeft(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltUpLeft({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x01, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltUpRight(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltUpRight({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x02, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDownLeft(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltDownLeft({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x01, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDownRight(short panSpeed, short tiltSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(panSpeed);
            byte bytetiltSpeed = ConvertSpeed(tiltSpeed);

            PTZLogger.Log.Info("PanTiltDownRight({0},{1})", panSpeed, tiltSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, bytetiltSpeed, 0x02, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltStop()
        {
            PTZLogger.Log.Info("PanTiltStop()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x03, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data);
                m_SocketClient.SendData(data);
            }
        }
    }
}
