using BMDSwitcherAPI;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public bool isConnected() {
            return is_connected;
        }
        public void connect()
        {
            

            if (is_connecting || is_connected)
            {
                return;
            }
            // Create switcher discovery object
            atem_discovery = new CBMDSwitcherDiscovery();
            // Connect to switcher
            _BMDSwitcherConnectToFailure failureReason = 0;
            is_connecting = true;
            Task m_Task = Task.Factory.StartNew(() => { try
                {
                    atem_discovery.ConnectTo(this.atem_ip, out this.atem_switcher, out failureReason);
                }
                catch (COMException comEx)
                {
                    Log.Error("Failed to connect to ATM, got error: " + comEx.Message);
                    atem_switcher = null;
                }
                finally {
                    is_connected = failureReason == 0 && atem_switcher != null;
                    is_connecting = false;
                }
            });
            return;
        }
        public bool waitForConnection() {
            while (is_connecting) {
                Thread.Sleep(10);
            }
            return is_connected;
        }

        public void disconnect()
        {
            if (this.atem_switcher != null) {
               // atem_switcher.RemoveCallback();   TODO: implement the callback used, remove it here

            }
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
