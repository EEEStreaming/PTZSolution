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
                
                m_SocketClient.OpenChanel(this);
            }
        }

        public void AAA()
        {
            if (m_SocketClient != null && m_SocketClient.Connected)
                m_SocketClient.SendData(new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x06, 0x01, 0x17, 0x17, 0x03, 0x01, 0xFF });

        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Tally(bool ledRed, bool ledGreen)
        {
            throw new NotImplementedException();
        }
    }
    }
}
