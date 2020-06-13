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
