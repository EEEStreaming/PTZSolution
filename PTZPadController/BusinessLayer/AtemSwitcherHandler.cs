using BMDSwitcherAPI;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
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
        private SwitcherCallback atem_callback;
        private volatile List<IBMDSwitcherInput> inputs;

        private string atem_ip;
        volatile private bool is_connecting;
        volatile private bool is_connected;

        public AtemSwitcherHandler(string ip)
        {
            this.atem_ip = ip;
            this.is_connecting = false;
            this.is_connected = false;
            this.atem_callback = new SwitcherCallback();
            this.inputs = new List<IBMDSwitcherInput>(4);
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

        public void setPreviewSource(Source previewSource)
        {
            connect();
            switch (previewSource)
            {
                case Source.Source_1:
                    break;
                case Source.Source_2:
                    break;
                case Source.Source_3:
                    break;
                case Source.Source_4:
                    break;
            }
        }

        public void setProgramSource(Source programCamera)
        {
            throw new NotImplementedException();
        }

        private void switcherConnected()
        {
            atem_switcher.AddCallback(atem_callback);
            inputs = this.SwitcherInputs
                .Where((i, ret) =>
                {
                    _BMDSwitcherPortType type;
                    i.GetPortType(out type);
                    return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
                }) as List<IBMDSwitcherInput>;
        }

        private IEnumerable<IBMDSwitcherMixEffectBlock> MixEffectBlocks
        {
            get
            {
                // Create a mix effect block iterator
                IntPtr meIteratorPtr;
                atem_switcher.CreateIterator(typeof(IBMDSwitcherMixEffectBlockIterator).GUID, out meIteratorPtr);
                IBMDSwitcherMixEffectBlockIterator meIterator = Marshal.GetObjectForIUnknown(meIteratorPtr) as IBMDSwitcherMixEffectBlockIterator;
                if (meIterator == null)
                    yield break;

                // Iterate through all mix effect blocks
                while (true)
                {
                    IBMDSwitcherMixEffectBlock me;
                    meIterator.Next(out me);

                    if (me != null)
                        yield return me;
                    else
                        yield break;
                }
            }
        }

        private IEnumerable<IBMDSwitcherInput> SwitcherInputs
        {
            get
            {
                // Create an input iterator
                IntPtr inputIteratorPtr;
                atem_switcher.CreateIterator(typeof(IBMDSwitcherInputIterator).GUID, out inputIteratorPtr);
                IBMDSwitcherInputIterator inputIterator = Marshal.GetObjectForIUnknown(inputIteratorPtr) as IBMDSwitcherInputIterator;
                if (inputIterator == null)
                    yield break;

                // Scan through all inputs
                while (true)
                {
                    IBMDSwitcherInput input;
                    inputIterator.Next(out input);

                    if (input != null)
                        yield return input;
                    else
                        yield break;
                }
            }
        }

        private class SwitcherCallback : IBMDSwitcherCallback
        {
            public void Notify(_BMDSwitcherEventType eventType, _BMDSwitcherVideoMode coreVideoMode)
            {
                throw new NotImplementedException();
            }
        }

    }
}
