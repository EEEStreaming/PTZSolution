using BMDSwitcherAPI;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{
    public delegate void SwitcherEventHandler(object sender, object args);
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

    public class AtemSwitcherParser : ISwitcherParser
    {
        private IBMDSwitcher atem_switcher;
        //private IBMDSwitcherCallback atem_callback;
        //private volatile List<IBMDSwitcherInput> inputs;
        private List<InputMonitor> m_inputHDMIMonitors = new List<InputMonitor>();
        private IBMDSwitcherMixEffectBlock firstMixEffectBlock;

        private MixEffectBlockMonitor m_mixEffectBlockMonitor;
        private SwitcherMonitor m_switcherMonitor;

        private string m_CurrentProgramName;
        private string m_CurrentPreviewName;

        private string atem_ip;
        private string lastCameraName;


        volatile private bool is_connecting;
        volatile private bool is_connected;
        private CancellationTokenSource m_Cancellation;

        public bool Connected { get { return is_connected; } }
        public bool IsConnecting { get { return is_connecting; } }

        #region Constructor/Initialisation/Connection
        public AtemSwitcherParser(string ip)
        {
            atem_ip = ip;
            is_connecting = false;
            is_connected = false;
            m_switcherMonitor = new SwitcherMonitor();

            // note: this invoke pattern ensures our callback is called in the main thread. We are making double
            // use of lambda expressions here to achieve this.
            // Essentially, the events will arrive at the callback class (implemented by our monitor classes)
            // on a separate thread. We must marshal these to the main thread, and we're doing this by calling
            // invoke on the Windows Forms object. The lambda expression is just a simplification.
            m_switcherMonitor.SwitcherDisconnected += new SwitcherEventHandler((s, a) => { if (App.Win!= null)App.Win.Dispatcher.Invoke(() => SwitcherDisconnected()); });

            m_mixEffectBlockMonitor = new MixEffectBlockMonitor();
            m_mixEffectBlockMonitor.ProgramInputChanged += new SwitcherEventHandler((s, a) => { if (App.Win != null) App.Win.Dispatcher.Invoke(() => OnProgramSourceChange()); });
            m_mixEffectBlockMonitor.PreviewInputChanged += new SwitcherEventHandler((s, a) => { if (App.Win != null) App.Win.Dispatcher.Invoke(() => OnPreviewSourceChange()); });
            //m_mixEffectBlockMonitor.TransitionFramesRemainingChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => UpdateTransitionFramesRemaining())));
            //m_mixEffectBlockMonitor.TransitionPositionChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => UpdateSliderPosition())));
            //m_mixEffectBlockMonitor.InTransitionChanged += new SwitcherEventHandler((s, a) => App.Win.Dispatcher.Invoke((Action)(() => OnInTransitionChanged())));

        }




        public void Connect()
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
                PTZLogger.Log.Debug("Connected to ATEM");
            }
            catch (COMException comEx)
            {
                PTZLogger.Log.Error(comEx, "Failed to connect to ATME");
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                switch (failureReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        PTZLogger.Log.Error("No response from Switcher");
                        //start a task to try to reconnect again.
                        if (m_Cancellation == null)
                            m_Cancellation = new CancellationTokenSource();
                        // Create the task to re-connect.
                        //Multi-thread get issue with ATEM SDK.
                        //var task = Task.Factory.StartNew(() =>
                        //{
                        //    if (!m_Cancellation.IsCancellationRequested)
                        //    {
                        //        Thread.Sleep(1000);
                        //        PTZLogger.Log.Info("Atem, {0}, try to re-connect ", atem_ip);
                        //        Connect();
                        //    }
                        //}, m_Cancellation.Token, TaskCreationOptions.None, TaskScheduler.Default);
                        //if (m_Cancellation.Token.IsCancellationRequested)
                        //{
                        //    m_Cancellation = null;
                        //    atem_switcher = null;
                        //}

                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        PTZLogger.Log.Error("Switcher has incompatible firmware");
                        break;
                    default:
                        PTZLogger.Log.Error("Connection failed for unknown reason");
                        break;
                }

                atem_switcher = null;
            }
            finally
            {
                if (failureReason == 0 && atem_switcher != null)
                {
                    if (App.Win != null)
                        App.Win.Dispatcher.Invoke(() => SwitcherConnected());
                    else
                        SwitcherConnected();
                    if (m_inputHDMIMonitors.Count >= 4)
                    {
                        is_connected = true;
                    }
                    else
                    {
                        is_connected = false;

                    }
                    Messenger.Default.Send(new NotificationMessage<ISwitcherParser>(this, NotificationSource.SwictcherConnected));
                }
                else
                {
                    is_connected = false;
                }
            }
            is_connecting = false;
        }


        public void Disconnect()
        {
            if (Connected)
            {
                SwitcherDisconnected();
            }

            if (m_Cancellation != null)
            {
                m_Cancellation.Cancel();
                m_Cancellation = null;
            }

        }

        private void SwitcherConnected()
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


            //Read current Preview and Program input
            m_CurrentProgramName = GetCameraProgramName();
            m_CurrentPreviewName = GetCameraPreviewName();
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
            is_connected = false;
            Messenger.Default.Send(new NotificationMessage<ISwitcherParser>(this, NotificationSource.SwictcherConnected));
        }


        #endregion
        private void OnInTransitionChanged()
        {
            //throw new NotImplementedException();
        }

        private void UpdateSliderPosition()
        {
           // throw new NotImplementedException();
        }

        private void UpdateTransitionFramesRemaining()
        {
          //  throw new NotImplementedException();
        }



        protected virtual void OnPreviewSourceChange()
        {
            long previewId;

            firstMixEffectBlock.GetPreviewInput(out previewId);
            lastCameraName = m_CurrentPreviewName;

            AtemSourceMessageArgs args = new AtemSourceMessageArgs();
            args.PreviousInputName = m_CurrentPreviewName;
            m_CurrentPreviewName = GetInputNameById(previewId);
            args.CurrentInputName = m_CurrentPreviewName;

            Messenger.Default.Send(new NotificationMessage<AtemSourceMessageArgs>(args, NotificationSource.PreviewSourceChanged));
        }

        protected virtual void OnProgramSourceChange()
        {
            long programId;

            firstMixEffectBlock.GetProgramInput(out programId);

            AtemSourceMessageArgs args = new AtemSourceMessageArgs();
            args.PreviousInputName = m_CurrentProgramName;
            m_CurrentProgramName = GetInputNameById(programId);
            args.CurrentInputName = m_CurrentProgramName;
            Messenger.Default.Send(new NotificationMessage<AtemSourceMessageArgs>(args, NotificationSource.ProgramSourceChanged));
        }

        public void SetPreviewSource(string cameraName)
        {
            if (is_connected)
            {
                //if (cameraName != m_CurrentProgramName && cameraName != m_CurrentPreviewName)
                //{
                    var input = m_inputHDMIMonitors.FirstOrDefault(x => x.InputName == cameraName);
                    if (input != null)
                    {
                        firstMixEffectBlock.SetPreviewInput(input.InputId);
                    }
                //}
            }
        }

        public void NextSwitcherPreview(List<string> cameraNames)
        {
            if (is_connected)
            {
                //we need this variable only if we have 4 cameras. With 3 we don't need it.
                if (cameraNames.Count < 4)
                    lastCameraName = String.Empty;

                //Push the next free camera to preview
                foreach (var name in cameraNames)
                {
                    if (name != m_CurrentProgramName && name != m_CurrentPreviewName && name != lastCameraName)
                    {
                        var input = m_inputHDMIMonitors.FirstOrDefault(x => x.InputName == name);
                        if (input != null)
                        {
                            firstMixEffectBlock.SetPreviewInput(input.InputId);
                        }
                    }
                }
            }
        }

        public void StartTransition(TransitionEnum transition)
        {
            if (is_connected)
            {
                if (transition == TransitionEnum.Cut)
                    firstMixEffectBlock.PerformCut();
                else if (transition == TransitionEnum.Mix)
                    firstMixEffectBlock.PerformAutoTransition();
            }
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
            return string.Empty;
        }

        public string GetCameraPreviewName()
        {
            long previewId;

            firstMixEffectBlock.GetPreviewInput(out previewId);
            var input = m_inputHDMIMonitors.FirstOrDefault(x => x.InputId == previewId);
            if (input != null)
                return input.InputName;
            return string.Empty;
        }

        public bool FindCameraName(string cameraName)
        {
            return m_inputHDMIMonitors.FirstOrDefault(x => x.InputName == cameraName) != null;
            
        }


    }
}
