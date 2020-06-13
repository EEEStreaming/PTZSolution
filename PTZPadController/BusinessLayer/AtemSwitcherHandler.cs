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
    class AtemSwitcherHandler : IAtemSwitcherHandler, IBMDSwitcherInputCallback
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

        public void onPreviewSourceChange(Source previewSource)
        {
            throw new NotImplementedException();
        }

        public void onProgramSourceChange(Source source)
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
            // Initialize ATEM Inputs
            inputs = this.SwitcherInputs
                .Where((i, ret) =>
                {
                    _BMDSwitcherPortType type;
                    i.GetPortType(out type);
                    
                    return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
                }).ToList();
            // Register callbacks for state changes on Inputs
            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                input.AddCallback(this);
            }
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
        
        public void Notify(_BMDSwitcherInputEventType eventType)
        {
            IBMDSwitcherMixEffectBlock mixEffectBlock = this.MixEffectBlocks.First();
            long currentInput = -1;
            switch (eventType)
            {
                case _BMDSwitcherInputEventType.bmdSwitcherInputEventTypeIsPreviewTalliedChanged:
                    mixEffectBlock.GetPreviewInput(out currentInput);
                    onPreviewSourceChange(GetInputById(currentInput));
                    break;
                case _BMDSwitcherInputEventType.bmdSwitcherInputEventTypeIsProgramTalliedChanged:
                    mixEffectBlock.GetProgramInput(out currentInput);
                    onProgramSourceChange(GetInputById(currentInput));
                    break;
            }
        }

        private Source GetInputById (long input_id)
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                long currentInputId;
                inputs[i].GetInputId(out currentInputId);
                if (currentInputId == input_id)
                {
                    return (Source) i;
                }
            }
            throw new ArgumentException("No input found");
        }
    }
}
