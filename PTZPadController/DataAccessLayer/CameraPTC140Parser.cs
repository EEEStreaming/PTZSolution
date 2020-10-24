using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace PTZPadController.DataAccessLayer
{

    public class CameraPTC140Parser : ICameraParser, IClientCallback
    {
        private ISocketParser m_SocketClient;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Connected { get { return (m_SocketClient != null) && (m_SocketClient.Connected); } }

        public string CameraName { get { return m_SocketClient?.SocketName; } }

        protected EFocusMode eFocusMode { get; private set; }


        #region CompletionMessage and Gets
        public void CompletionMessage(string message)
        {
            PTZLogger.Log.Info("CompletionMessage {0} Message reçu {1}", CameraName, message);
            if (message == "00-08-90-41-FF-90-51-FF")
            {
                PTZLogger.Log.Debug("CompletionMessage {0} Message ok", CameraName);
            }
            else if (message == "00-08-90-60-02-FF")
            {
                PTZLogger.Log.Debug("CompletionMessage {0} Syntax Error", CameraName);               
            }
            else if (message == "00-08-90-61-41-FF")
            {
                PTZLogger.Log.Debug("CompletionMessage {0} Command Not Executable", CameraName);
            }
            else if (message == "00-06-90-50-02-FF")
            {
                eFocusMode = EFocusMode.Auto;
                PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            }
            else if (message == "00-06-90-50-03-FF")
            {
                eFocusMode = EFocusMode.Manual;
                PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            }
            else if (message == "00-06-90-50-04-FF")
            {
                eFocusMode = EFocusMode.OnePush;
                PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            }
            else
            {
                PTZLogger.Log.Debug("CompletionMessage {0} message non traité: {1}", CameraName, message);
            }
            //TODO
        }

        public EFocusMode FocusMode()
        {
            PTZLogger.Log.Info("FocusMode()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                eFocusMode = EFocusMode.Unknown;
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x09, 0x04, 0x38, 0xFF };
                PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);

                short nCount = 0;
                while (eFocusMode == EFocusMode.Unknown && nCount < 50)
                {
                    Thread.Sleep(20);
                    nCount++;
                }
                PTZLogger.Log.Info("FocusMode() {0} FocusMode:{1}", CameraName, eFocusMode);
                return eFocusMode;
            }
            PTZLogger.Log.Error("FocusMode() {0} pas de réponse pour FocusMode dans le temps imparti", CameraName);
            return EFocusMode.Unknown;
        }

        #endregion

        #region Init
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
        #endregion

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

        #region position
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltStop()
        {
            PTZLogger.Log.Info("PanTiltStop()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x03, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltHome()
        {
            PTZLogger.Log.Info("PanTiltHome()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x07, 0x81, 0x01, 0x06, 0x04, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }
        #endregion

        #region Zoom
        public void ZoomStop()
        {
            PTZLogger.Log.Info("ZoomStop()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x07, 0x00, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void ZoomTele()
        {
            PTZLogger.Log.Info("ZoomTele()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x07, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void ZoomWide()
        {
            PTZLogger.Log.Info("ZoomWide()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x07, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public byte ConvertZoom(decimal zoomSpeed, short add)
        {
            short speed = 0;
            decimal centre = (decimal)0.5;
            decimal multMax = (decimal)30;
            if (zoomSpeed > centre && zoomSpeed <= 1)
            {
                speed = (byte)( (zoomSpeed - centre )* multMax);
            }
            else if (zoomSpeed < centre && zoomSpeed >= 0)
            {
                speed = (byte)((centre - zoomSpeed) * multMax);
            }

            return (byte)(add + speed);
        }

        public void ZoomTele(decimal zoomSpeed)
        {
            byte speed = ConvertZoom(zoomSpeed, 0x20);
            PTZLogger.Log.Info("ZoomTele({0}) -> {1}", zoomSpeed, speed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x07, speed, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void ZoomWide(decimal zoomSpeed)
        {
            byte speed = ConvertZoom(zoomSpeed, 0x30);
            PTZLogger.Log.Info("ZoomWide({0}) -> {1}", zoomSpeed, speed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x07, speed, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }
        #endregion

        #region Focus
        public void FocusModeAuto()
        {
            PTZLogger.Log.Info("FocusModeAuto()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x38, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusModeManual()
        {
            PTZLogger.Log.Info("FocusModeManual()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x38, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusModeOnePush()
        {
            PTZLogger.Log.Info("FocusModeOnePush()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x38, 0x04, 0xFF };
                PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusOnePushTrigger()
        {
            PTZLogger.Log.Info("FocusOnePushTrigger()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x18, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }
        #endregion

        private byte ConvertMemory(short memory)
        {
            byte byteMemory = (byte)(memory > 15 ? 15 : memory < 0 ? 0 : memory);
            return byteMemory;
        }

        public void CameraMemoryReset(short memory)
        {
            byte byteMemory = ConvertMemory(memory);
            PTZLogger.Log.Info("CameraMemoryReset()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x09, 0x81, 0x01, 0x04, 0x3F, 0x00, byteMemory,0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void CameraMemorySet(short memory)
        {
            byte byteMemory = ConvertMemory(memory);
            PTZLogger.Log.Info("CameraMemorySet()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x09, 0x81, 0x01, 0x04, 0x3F, 0x01, byteMemory, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void CameraMemoryRecall(short memory)
        {
            byte byteMemory = ConvertMemory(memory);
            PTZLogger.Log.Info("CameraMemoryRecall()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x09, 0x81, 0x01, 0x04, 0x3F, 0x02, byteMemory, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }


        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
