using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.BusinessLayer
{
    public class GamePadHandler : IGamePadHandler
    {
        private IHIDParser m_HidParser;
        private IPTZManager m_PtzManager;

        public void ConnectTo()
        {
            m_HidParser.ExecuteAsync().ConfigureAwait(true);
        }

        public void Disconnect()
        {
            m_HidParser.StopAsync();
        }

        public void Initialize(IHIDParser hidParser, IPTZManager manager)
        {
            m_HidParser = hidParser;
            m_PtzManager = manager;
        }
    }
}
