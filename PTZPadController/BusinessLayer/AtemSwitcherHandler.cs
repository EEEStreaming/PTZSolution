using BMDSwitcherAPI;
using NLog.Fluent;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace PTZPadController.BusinessLayer
{
    public delegate void SwitcherEventHandler(object sender, object args);
    public class SourceArgs : EventArgs
    {
        public string CurrentInputName { get; set; }
    }

    class SwitcherMonitor : IBMDSwitcherCallback
    {
        // Events:
        public event SwitcherEventHandler SwitcherDisconnected;

        public SwitcherMonitor()
        {
        }

        void IBMDSwitcherCallback.Notify(_BMDSwitcherEventType eventType, _BMDSwitcherVideoMode coreVideoMode)
        {
            if (eventType == _BMDSwitcherEventType.bmdSwitcherEventTypeDisconnected)
            {
                if (SwitcherDisconnected != null)
                    SwitcherDisconnected(this, null);
            }
        }
    }

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

    class InputMonitor : IBMDSwitcherInputCallback
    {
        // Events:
        public event SwitcherEventHandler LongNameChanged;

        private IBMDSwitcherInput m_input;
        public IBMDSwitcherInput Input { get { return m_input; } }

        public string InputName { get; set; }
        public long InputId { get; set; }

        public InputMonitor(IBMDSwitcherInput input)
        {
            m_input = input;
            string inputName;
            m_input.GetLongName(out inputName);
            InputName = inputName;
            long inputId;
            m_input.GetInputId(out inputId);

            InputId = inputId;
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


    class AtemSwitcherHandler : IAtemSwitcherHandler
    {
        private volatile IBMDSwitcher atem_switcher;
        //private IBMDSwitcherCallback atem_callback;
        //private volatile List<IBMDSwitcherInput> inputs;
        private List<InputMonitor> m_inputHDMIMonitors = new List<InputMonitor>();
        private IBMDSwitcherMixEffectBlock firstMixEffectBlock;

        private MixEffectBlockMonitor m_mixEffectBlockMonitor;
        private SwitcherMonitor m_switcherMonitor;

        private string atem_ip;
        volatile private bool is_connecting;
        volatile private bool is_connected;

        public event EventHandler<SourceArgs> PreviewSourceChanged;
        public event EventHandler<SourceArgs> ProgramSourceChanged;


        public AtemSwitcherHandler(string ip)
        {
            this.atem_ip = ip;
            this.is_connecting = false;
            this.is_connected = false;
            m_switcherMonitor = new SwitcherMonitor();

            // note: this invoke pattern ensures our callback is called in the main thread. We are making double
            // use of lambda expressions here to achieve this.
            // Essentially, the events will arrive at the callback class (implemented by our monitor classes)
            // on a separate thread. We must marshal these to the main thread, and we're doing this by calling
            // invoke on the Windows Forms object. The lambda expression is just a simplification.
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => SwitcherDisconnected())));

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => onProgramSourceChange())));
            m_mixEffectBlockMonitor.PreviewInputChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => onPreviewSourceChange())));
            //m_mixEffectBlockMonitor.TransitionFramesRemainingChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => UpdateTransitionFramesRemaining())));
            //m_mixEffectBlockMonitor.TransitionPositionChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => UpdateSliderPosition())));
            //m_mixEffectBlockMonitor.InTransitionChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => OnInTransitionChanged())));

        }

        private void OnInTransitionChanged()
        {
            throw new NotImplementedException();
        }

        private void UpdateSliderPosition()
        {
            throw new NotImplementedException();
        }

        private void UpdateTransitionFramesRemaining()
        {
            throw new NotImplementedException();
        }


        private void SwitcherDisconnected()
        {
            // Remove all input monitors, remove callbacks
            foreach (InputMonitor inputMon in m_inputHDMIMonitors)
            {
                inputMon.Input.RemoveCallback(inputMon);
                inputMon.LongNameChanged -= new SwitcherEventHandler(OnInputLongNameChanged);
            }
            m_inputHDMIMonitors.Clear();

            if (firstMixEffectBlock != null)
            {
                // Remove callback
                firstMixEffectBlock.RemoveCallback(m_mixEffectBlockMonitor);

                // Release reference
                firstMixEffectBlock = null;
            }

            if (atem_switcher != null)
            {
                // Remove callback:
                atem_switcher.RemoveCallback(m_switcherMonitor);

                // release reference:
                atem_switcher = null;
            }

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
                        if (m_inputHDMIMonitors.Count >=4 )
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
            //if (this.atem_switcher != null) {
            //    atem_switcher.RemoveCallback(atem_callback);
            //    atem_switcher = null;
            //    is_connected = false;
            //}
        }

        protected virtual void onPreviewSourceChange()
        {
            long previewId;

            firstMixEffectBlock.GetPreviewInput(out previewId);

            EventHandler<SourceArgs> handler = PreviewSourceChanged;
            SourceArgs args = new SourceArgs();
            args.CurrentInputName = GetInputNameById(previewId);
            handler?.Invoke(this, args);
        }

        protected virtual void onProgramSourceChange()
        {
            long programId;

            firstMixEffectBlock.GetProgramInput(out programId);

            EventHandler<SourceArgs> handler = ProgramSourceChanged;
            SourceArgs args = new SourceArgs();
            args.CurrentInputName = GetInputNameById(programId);
            handler?.Invoke(this, args);
        }

        public void setPreviewSource(Source previewSource)
        {
            connect();
            
            //firstMixEffectBlock.SetPreviewInput(inputId);
        }

        public void setProgramSource(Source programCamera)
        {
            throw new NotImplementedException();
        }

        private void switcherConnected()
        {
            // Install SwitcherMonitor callbacks:
            atem_switcher.AddCallback(m_switcherMonitor);

            // We create input monitors for each input. To do this we iterate over all inputs:
            // This will allow us to update the combo boxes when input names change:
            IBMDSwitcherInputIterator inputIterator = null;
            IntPtr inputIteratorPtr;
            Guid inputIteratorIID = typeof(IBMDSwitcherInputIterator).GUID;
            atem_switcher.CreateIterator(ref inputIteratorIID, out inputIteratorPtr);
            if (inputIteratorPtr != null)
            {
                inputIterator = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(inputIteratorPtr);
            }

            if (inputIterator != null)
            {
                IBMDSwitcherInput input;
                _BMDSwitcherPortType inputType;

                inputIterator.Next(out input);
                while (input != null)
                {
                    input.GetPortType(out inputType);
                    //filter we monitor only HDMI Source
                    if (inputType == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal)
                    {
                        InputMonitor newInputMonitor = new InputMonitor(input);
                        input.AddCallback(newInputMonitor);
                        newInputMonitor.LongNameChanged += new SwitcherEventHandler(OnInputLongNameChanged);

                        m_inputHDMIMonitors.Add(newInputMonitor);
                    }

                    inputIterator.Next(out input);
                }
            }

            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            firstMixEffectBlock = null;

            IBMDSwitcherMixEffectBlockIterator meIterator = null;
            IntPtr meIteratorPtr;
            Guid meIteratorIID = typeof(IBMDSwitcherMixEffectBlockIterator).GUID;
            atem_switcher.CreateIterator(ref meIteratorIID, out meIteratorPtr);
            if (meIteratorPtr != null)
            {
                meIterator = (IBMDSwitcherMixEffectBlockIterator)Marshal.GetObjectForIUnknown(meIteratorPtr);
            }

            if (meIterator == null)
                return;

            if (meIterator != null)
            {
                meIterator.Next(out firstMixEffectBlock);
            }

            if (firstMixEffectBlock == null)
            {
                PTZLogger.Log.Error("Unexpected: Could not get first mix effect block", "Error");
                return;
            }

            // Install MixEffectBlockMonitor callbacks:
            firstMixEffectBlock.AddCallback(m_mixEffectBlockMonitor);


            //// Initialize ATEM Inputs
            //inputs = this.SwitcherInputs
            //    .Where((i, ret) =>
            //    {
            //        _BMDSwitcherPortType type;
            //        i.GetPortType(out type);
                    
            //        return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
            //    }).ToList();
            //// Register callbacks for state changes on Inputs
            //for (var i = 0; i < inputs.Count; i++)
            //{
            //    var input = inputs[i];
            //    input.AddCallback(this);
            //}
        }

        private void OnInputLongNameChanged(object sender, object args)
        {
            throw new NotImplementedException();
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

        private string GetInputNameById(long input_id)
        {
            for (var i = 0; i < m_inputHDMIMonitors.Count; i++)
            {
                long currentInputId;
                m_inputHDMIMonitors[i].Input.GetInputId(out currentInputId);
                if (currentInputId == input_id)
                {
                    return m_inputHDMIMonitors[i].InputName;
                }
            }
            throw new ArgumentException("No input found");
        }

        public string GetCameraProgramName()
        {
            long programId;

            firstMixEffectBlock.GetProgramInput(out programId);
            var input = m_inputHDMIMonitors.FirstOrDefault(x => x.InputId == programId);
            if (input != null)
                return input.InputName;
            return String.Empty;
        }

        public string GetCameraPreviewName()
        {
            long previewId;

            firstMixEffectBlock.GetPreviewInput(out previewId);
            var input = m_inputHDMIMonitors.FirstOrDefault(x => x.InputId == previewId);
            if (input != null)
                return input.InputName;
            return String.Empty;
        }
    }
}
