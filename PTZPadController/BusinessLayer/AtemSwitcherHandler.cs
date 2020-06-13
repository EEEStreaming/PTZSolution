using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    class AtemSwitcherHandler : IAtemSwitcherHandler
    {
        private IBMDSwitcherDiscovery atem_discovery;
        private IBMDSwitcher atem_switcher;
        private string atem_ip;
        volatile private bool is_connecting;
        volatile private bool is_connected;

        public AtemSwitcherHandler(string ip)
        {
            this.atem_ip = ip;
            this.is_connecting = false;
            this.is_connected = false;
        }

        public AtemSwitcherHandler() : this("192.168.1.135") { } // TODO : read ip from config.

        public Task connect()
        {
            // Create switcher discovery object
            atem_discovery = new CBMDSwitcherDiscovery();

            if (atem_switcher != null)
            {
                return Task.Factory.StartNew(() => { return; });
            }

            // Connect to switcher
            _BMDSwitcherConnectToFailure failureReason;
            is_connecting = true;
            Task m_Task = Task.Factory.StartNew(() => { atem_discovery.ConnectTo(this.atem_ip, out atem_switcher, out failureReason);
                is_connecting = false;
            });
            return m_Task;
        }

        public void disconnect()
        {
            throw new NotImplementedException();
        }

        public void onPreviewSourceChange()
        {
            throw new NotImplementedException();
        }

        public void onProgramSourceChange()
        {
            throw new NotImplementedException();
        }

        public void setPreviewSource(Camera previewCamera)
        {
            throw new NotImplementedException();
        }

        public void setProgramSource(Camera programCamera)
        {
            throw new NotImplementedException();
        }
    }
}
