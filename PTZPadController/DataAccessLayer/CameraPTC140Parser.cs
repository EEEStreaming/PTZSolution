using GalaSoft.MvvmLight.Messaging;
using PTZPadController.Messages;
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

        protected ECameraFocusMode eFocusMode { get; private set; }


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
            //else if (message == "00-06-90-50-02-FF")
            //{
            //    eFocusMode = ECameraFocusMode.Auto;
            //    PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            //}
            //else if (message == "00-06-90-50-03-FF")
            //{
            //    eFocusMode = ECameraFocusMode.Manual;
            //    PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            //}
            //else if (message == "00-06-90-50-04-FF")
            //{
            //    eFocusMode = ECameraFocusMode.OnePush;
            //    PTZLogger.Log.Debug("CompletionMessage {0}  FocusMode:{1}", CameraName, eFocusMode);
            //}
            else
            {
                PTZLogger.Log.Debug("CompletionMessage {0} message non traité: {1}", CameraName, message);
            }
            //TODO
        }

        //public void FocusMode()
        //{
        //    PTZLogger.Log.Info("FocusMode()");
        //    if (m_SocketClient != null && m_SocketClient.Connected)
        //    {
        //        eFocusMode = ECameraFocusMode.Unknown;
        //        byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x09, 0x04, 0x38, 0xFF };
        //        //PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
        //        m_SocketClient.SendData(data);

        //        PTZLogger.Log.Info("FocusMode() {0} FocusMode:{1}", CameraName, eFocusMode);
        //    }
        //}

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
        private byte ConvertSpeed(short speed,bool pan)
        {
            short max = pan ? (short)0x18 : (short)0x14;

            byte byteSpeed = (byte) (speed*max/10.0);
            return byteSpeed;
        }

        public void PanTiltUp(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltUp({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x03, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDown(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltDown({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x03, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltLeft(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltLeft({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x01, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltRight(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltRight({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x02, 0x03, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltUpLeft(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltUpLeft({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x01, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltUpRight(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltUpRight({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x02, 0x01, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDownLeft(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltDownLeft({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x01, 0x02, 0xFF };
                PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        public void PanTiltDownRight(short moveSpeed)
        {
            byte bytePanSpeed = ConvertSpeed(moveSpeed, true);
            byte byteTiltSpeed = ConvertSpeed(moveSpeed, false);

            PTZLogger.Log.Info("PanTiltDownRight({0})", moveSpeed);
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, bytePanSpeed, byteTiltSpeed, 0x02, 0x02, 0xFF };
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

        public byte ConvertZoom(short zoomSpeed, short add)
        {
            byte speed = (byte)((double)zoomSpeed*15/10.0);
            return (byte)(add + speed);
        }

        public void ZoomTele(short zoomSpeed)
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

        public void ZoomWide(short zoomSpeed)
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
                //PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusModeManual()
        {
            PTZLogger.Log.Info("FocusModeManual()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x38, 0x03, 0xFF };
               // PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusModeOnePush()
        {
            PTZLogger.Log.Info("FocusModeOnePush()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x38, 0x04, 0xFF };
                //PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
                m_SocketClient.SendData(data);
            }
        }

        public void FocusOnePushTrigger()
        {
            PTZLogger.Log.Info("FocusOnePushTrigger()");
            if (m_SocketClient != null && m_SocketClient.Connected)
            {
                byte[] data = new byte[] { 0x00, 0x08, 0x81, 0x01, 0x04, 0x18, 0x01, 0xFF };
                //PTZLogger.Log.Debug("data:{0}", BitConverter.ToString(data));
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
                //PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                //PTZLogger.Log.Debug("data:{0}", data.ToString());
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
                //PTZLogger.Log.Debug("data:{0}", data.ToString());
                m_SocketClient.SendData(data);
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
