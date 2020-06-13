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
    public delegate void SwitcherEventHandler(object sender, object args);
    class AtemSwitcherHandler : IAtemSwitcherHandler
    {
        private volatile IBMDSwitcher atem_switcher;
        private IBMDSwitcherCallback atem_callback;
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
            
            // Connect to switcher
            _BMDSwitcherConnectToFailure failureReason = 0;
            is_connecting = true;
            try
                {
                    CBMDSwitcherDiscovery atem_discovery = new CBMDSwitcherDiscovery();
                    atem_discovery.ConnectTo(atem_ip, out atem_switcher, out failureReason);
                    Log.Debug("Connected to ATEM");
                }
                catch (COMException comEx)
                {
                    Log.Error("Failed to connect to ATM, got error: " + comEx.Message);
                    atem_switcher = null;
                }
                finally
                {
                    if (failureReason == 0 && atem_switcher != null)
                    {
                        switcherConnected();
                        if (inputs != null && inputs[0] != null && inputs[1] != null && inputs[2] != null && inputs[3] != null)
                        {
                            is_connected = true;
                        }
                        else
                        {
                            is_connected = false;

                        }
                    }
                    else
                    {
                        is_connected = false;
                    }
                    is_connecting = false;
                }
        }
        public bool waitForConnection() {
            while (is_connecting) {
                Console.WriteLine("sleeping for connexion");
                Thread.Sleep(10);
            }
            return is_connected;
        }

        public void disconnect()
        {
            if (this.atem_switcher != null) {
                atem_switcher.RemoveCallback(atem_callback);
                atem_switcher = null;
                is_connected = false;
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
            IBMDSwitcherMixEffectBlock mixEffectBlock = this.MixEffectBlocks.First();
            inputs.ElementAt((int)previewSource).GetInputId(out long inputId);
            mixEffectBlock.SetPreviewInput(inputId);
        }

        public void setProgramSource(Source programCamera)
        {
            throw new NotImplementedException();
        }

        private void switcherConnected()
        {
            // atem_switcher.AddCallback(atem_callback);
            inputs = this.SwitcherInputs
                .Where((i, ret) =>
                {
                    _BMDSwitcherPortType type;
                    i.GetPortType(out type);
                    return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
                }).ToList();
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
        /// <summary>
        /// Thank's http://difanet.jamu.cz/az_vyuka/manualy/Blackmagic_ATEM_studio/ATEM_Blackmagic_OSX10.13/ATEMProduction_Studio4K/Blackmagic%20ATEM%20Switchers%20SDK%208.0.3/Windows/Samples/SwitcherPanelCSharp/SwitcherMonitors.cs
        /// </summary>
        class MixEffectBlockMonitor : IBMDSwitcherMixEffectBlockCallback
        {
            // Events:
            public event SwitcherEventHandler ProgramInputChanged;
            public event SwitcherEventHandler PreviewInputChanged;
            public event SwitcherEventHandler TransitionFramesRemainingChanged;
            public event SwitcherEventHandler TransitionPositionChanged;
            public event SwitcherEventHandler InTransitionChanged;

            public MixEffectBlockMonitor()
            {
            }

            void IBMDSwitcherMixEffectBlockCallback.Notify(_BMDSwitcherMixEffectBlockEventType eventType)
            {
                switch (eventType)
                {
                    case _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypeProgramInputChanged:
                        if (ProgramInputChanged != null)
                            ProgramInputChanged(this, null);
                        break;
                    case _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypePreviewInputChanged:
                        if (PreviewInputChanged != null)
                            PreviewInputChanged(this, null);
                        break;
                    case _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypeTransitionFramesRemainingChanged:
                        if (TransitionFramesRemainingChanged != null)
                            TransitionFramesRemainingChanged(this, null);
                        break;
                    case _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypeTransitionPositionChanged:
                        if (TransitionPositionChanged != null)
                            TransitionPositionChanged(this, null);
                        break;
                    case _BMDSwitcherMixEffectBlockEventType.bmdSwitcherMixEffectBlockEventTypeInTransitionChanged:
                        if (InTransitionChanged != null)
                            InTransitionChanged(this, null);
                        break;
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
        class InputMonitor : IBMDSwitcherInputCallback
        {
            // Events:
            public event SwitcherEventHandler LongNameChanged;

            private IBMDSwitcherInput m_input;
            public IBMDSwitcherInput Input { get { return m_input; } }

            public InputMonitor(IBMDSwitcherInput input)
            {
                m_input = input;
            }

            void IBMDSwitcherInputCallback.Notify(_BMDSwitcherInputEventType eventType)
            {
                switch (eventType)
                {
                    case _BMDSwitcherInputEventType.bmdSwitcherInputEventTypeLongNameChanged:
                        if (LongNameChanged != null)
                            LongNameChanged(this, null);
                        break;
                }
            }
        }

    }
}
