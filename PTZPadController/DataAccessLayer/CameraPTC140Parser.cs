using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.DataAccessLayer
{
    public class CameraPTC140Parser : ICameraParser, IClientCallback
    {
        private ISocketParser m_SocketClient;

        public void CompletionMessage()
        {
            throw new NotImplementedException();
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

        //public void test1()
        //{

        //    if (m_SocketClient != null && m_SocketClient.Connected)
        //    {
        //        m_SocketClient.SendData();
        //    }
        //}
    }
}
