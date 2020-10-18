using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace PTZPadController.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PTZMainViewModel : ViewModelBase
    {
        private readonly IPTZManager m_PtzManager;
        private DeviceItemViewModel _Switcher;
        private DeviceItemViewModel _Pad;
        private ImageSource _Preset1Image;
        private ImageSource _Preset2Image;
        private ImageSource _Preset3Image;
        private ImageSource _Preset4Image;
        private ImageSource _Preset5Image;
        private ImageSource _Preset6Image;
        private ImageSource _Preset7Image;
        private ImageSource _Preset8Image;
        private PresetStatusEnum _PresetStatus;
        private Object _PresetState;
        private CancellationTokenSource _PresetCancellationToken;
        private Task _currentWaitingTask;

        public DeviceItemViewModel Switcher {
            get { return _Switcher; }
            set
            {
                if (_Switcher == value) return;
                _Switcher = value;
                RaisePropertyChanged("Switcher");
            }
        }

        public DeviceItemViewModel Pad
        {
            get { return _Pad; }
            set
            {
                if (_Pad == value) return;
                _Pad = value;
                RaisePropertyChanged("Pad");
            }
        }

        
        public ImageSource Preset1Image
        {
            get { return _Preset1Image; }
            set
            {
                if (_Preset1Image == value) return;
                _Preset1Image = value;
                RaisePropertyChanged("Preset1Image");
            }
        }

        public ImageSource Preset2Image
        {
            get { return _Preset2Image; }
            set
            {
                if (_Preset2Image == value) return;
                _Preset2Image = value;
                RaisePropertyChanged("Preset2Image");
            }
        }

        public ImageSource Preset3Image
        {
            get { return _Preset3Image; }
            set
            {
                if (_Preset3Image == value) return;
                _Preset3Image = value;
                RaisePropertyChanged("Preset3Image");
            }
        }

        public ImageSource Preset4Image
        {
            get { return _Preset4Image; }
            set
            {
                if (_Preset4Image == value) return;
                _Preset4Image = value;
                RaisePropertyChanged("Preset4Image");
            }
        }
        public ImageSource Preset5Image
        {
            get { return _Preset5Image; }
            set
            {
                if (_Preset5Image == value) return;
                _Preset5Image = value;
                RaisePropertyChanged("Preset5Image");
            }
        }

        public ImageSource Preset6Image
        {
            get { return _Preset6Image; }
            set
            {
                if (_Preset6Image == value) return;
                _Preset6Image = value;
                RaisePropertyChanged("Preset6Image");
            }
        }
        public ImageSource Preset7Image
        {
            get { return _Preset7Image; }
            set
            {
                if (_Preset7Image == value) return;
                _Preset7Image = value;
                RaisePropertyChanged("Preset7Image");
            }
        }

        public ImageSource Preset8Image
        {
            get { return _Preset8Image; }
            set
            {
                if (_Preset8Image == value) return;
                _Preset8Image = value;
                RaisePropertyChanged("Preset8Image");
            }
        }

        public PresetStatusEnum PresetStatus
        {
            get { return _PresetStatus; }
            set
            {
                if (_PresetStatus == value) return;
                _PresetStatus = value;
                RaisePropertyChanged("PresetStatus");
            }
        }
        

        #region Commands
        public ICommand CameraUp { get; private set; }
        public ICommand CameraUpLeft { get; private set; }
        public ICommand CameraUpRight { get; private set; }
        public ICommand CameraDown { get; private set; }
        public ICommand CameraDownLeft { get; private set; }
        public ICommand CameraDownRight { get; private set; }
        public ICommand CameraLeft { get; private set; }
        public ICommand CameraRight { get; private set; }
        public ICommand CameraPanStop { get; private set; }
        public ICommand CameraZoomTele { get; private set; }
        public ICommand CameraZoomWidde { get; private set; }
        public ICommand CameraZoomStop { get; private set; }
        public ICommand SwitcherCut { get; private set; }
        public ICommand SwitcherMix { get; private set; }
        public ICommand CameraPresetButtonUp { get; private set; }
        public ICommand CameraPresetButtonDown { get; private set; }
        public ICommand PresetImageChanged { get; private set; }
        #endregion

        public ObservableCollection<CameraViewModel> Cameras { get; set; }

        /// <summary>
        /// Initializes a new instance of the PTZMainViewModel class.
        /// </summary>
        public PTZMainViewModel(IPTZManager manager)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            _PresetState = new Object();
            m_PtzManager = manager;
            CameraUp = new RelayCommand(CameraUpExecute);
            CameraUpLeft = new RelayCommand(CameraUpLeftExecute);
            CameraUpRight = new RelayCommand(CameraUpRightExecute);
            CameraDown = new RelayCommand(CameraDownExecute);
            CameraDownLeft = new RelayCommand(CameraDownLeftExecute);
            CameraDownRight = new RelayCommand(CameraDownRightExecute);
            CameraLeft = new RelayCommand(CameraLeftExecute);
            CameraRight = new RelayCommand(CameraRightExecute);
            CameraPanStop = new RelayCommand(CameraPanStopExecute);
            CameraZoomTele = new RelayCommand(CameraZoomTeleExecute);
            CameraZoomWidde = new RelayCommand(CameraZoomWiddeExecute);
            CameraZoomStop = new RelayCommand(CameraZoomStopExecute);
            SwitcherCut = new RelayCommand(SwitcherCutExecute);
            SwitcherMix = new RelayCommand(SwitcherMixExecute);
            CameraPresetButtonUp = new RelayCommand<string>(CameraPresetButtonUpExecute);
            CameraPresetButtonDown = new RelayCommand<string>(CameraPresetButtonDownExecute);
            PresetImageChanged = new RelayCommand<PresetEventArgs>(PresetImageChangedExecute);
            Cameras = new ObservableCollection<CameraViewModel>();

            //Initialize viewModel
            CameraViewModel camvm;
            int cameraInput = 1;
            
            foreach (var cam in m_PtzManager.Cameras)
            {
                camvm = new CameraViewModel(m_PtzManager, cam.Parser, cameraInput);
                Cameras.Add(camvm);
                cameraInput++;
            }

            Switcher = new DeviceItemViewModel("ATEM");
            Pad = new DeviceItemViewModel("Pad");
            
            MessengerInstance.Register<NotificationMessage<CameraMessageArgs>>(this, CameraStatusChange);
            MessengerInstance.Register<NotificationMessage<ISwitcherParser>>(this, SwitcherNotification);
            MessengerInstance.Register<NotificationMessage<IHIDParser>>(this, GamePadNotification);


            //Startup the whole system 
            //Multi-thread get issue with ATEM SDK.
            m_PtzManager.StartUp();
            //Task.Factory.StartNew(()=>ptzManager.StartUp());

        }



        private void CameraPresetButtonUpExecute(string preset)
        {
            int iPreset;
            if (Int32.TryParse(preset, out iPreset))
            {
                if (_PresetStatus == PresetStatusEnum.WaitingForSetPreset)
                {
                    lock(_PresetState)
                    {
                        PresetStatus = PresetStatusEnum.CallPreset;
                        if (_PresetCancellationToken != null)
                        {
                            _PresetCancellationToken.Cancel();
                            _PresetCancellationToken = null;
                        }
                    }
                    m_PtzManager.CameraCallPreset(iPreset);
                    PTZLogger.Log.Debug("Preset {0} called", iPreset);

                }
                lock (_PresetState)
                {
                    PresetStatus = PresetStatusEnum.Idle;
                }

            }
        }

        private void CameraPresetButtonDownExecute(string preset)
        {
            int iPreset;
            if (Int32.TryParse(preset, out iPreset))
            {
                if (_PresetStatus == PresetStatusEnum.Idle)
                {
                    lock (_PresetState)
                    {
                        PresetStatus = PresetStatusEnum.WaitingForSetPreset;
                    }
                    PTZLogger.Log.Debug("Preset {0} button down", preset);

                    if (_PresetCancellationToken == null)
                        _PresetCancellationToken = new CancellationTokenSource();
                    Task.Factory.StartNew(() =>
                    {
                        _currentWaitingTask = Task.Delay(2000, _PresetCancellationToken.Token).ContinueWith((t) =>
                        {

                            if (!t.IsCanceled)//!_PresetCancellationToken.IsCancellationRequested)
                            {
                                if (_PresetStatus == PresetStatusEnum.WaitingForSetPreset)
                                {
                                    lock (_PresetState)
                                    {
                                        PresetStatus = PresetStatusEnum.SetPreset;
                                    }
                                    m_PtzManager.CameraSetPreset(iPreset);
                                    PTZLogger.Log.Debug("Preset {0} saved", iPreset);
                                    lock (_PresetState)
                                    {
                                        PresetStatus = PresetStatusEnum.PresetSaved;
                                    }
                                }
                            }
                            else
                                PTZLogger.Log.Debug("Save Preset {0} was cancled", iPreset);

                        }, _PresetCancellationToken.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);

                        _currentWaitingTask.Wait(3000, _PresetCancellationToken.Token);
                        _PresetCancellationToken = null;
                    });
                }
                else
                {
                    PTZLogger.Log.Warn("Something was wrong, Preset status {0} mode. Reset the state machine", _PresetStatus);
                    lock (_PresetState)
                    {
                        PresetStatus = PresetStatusEnum.Idle;
                        if (_PresetCancellationToken != null && !_PresetCancellationToken.IsCancellationRequested)
                            _PresetCancellationToken.Cancel();
                        _PresetCancellationToken = null;
                    }

                }
            }
        }


        private void SwitcherMixExecute()
        {
            //var msg = new NotificationMessage<TransitionMessageArgs>(new TransitionMessageArgs { Transition = TransitionEnum.Mix}, NotificationSource.SendTransition);
            //MessengerInstance.Send(msg);
            m_PtzManager.SendSwitcherTransition(TransitionEnum.Mix);
        }

        private void SwitcherCutExecute()
        {
            //var msg = new NotificationMessage<TransitionMessageArgs>(new TransitionMessageArgs { Transition = TransitionEnum.Cut }, NotificationSource.SendTransition);
            //MessengerInstance.Send(msg);
            m_PtzManager.SendSwitcherTransition(TransitionEnum.Cut);
        }

        private void CameraStatusChange(NotificationMessage<CameraMessageArgs> obj)
        {
            if (obj.Notification == NotificationSource.CameraStatusChanged)
            {
                PTZLogger.Log.Debug("Camera {0} status has changed to {1}",obj.Content.CameraName, obj.Content.Status);

                var camera = Cameras.FirstOrDefault(x => x.Name == obj.Content.CameraName);
                if (camera != null)
                {
                    camera.SourceStatus = obj.Content.Status;
                    if (obj.Content.Status == CameraStatusEnum.Preview)
                    {
                        //Update preset icon list.
                        UpdatePresetIcons();
                    }
                }
            }
        }

        private void UpdatePresetIcons()
        {
            System.Collections.Generic.List<PresetIconSettingModel> cfg = m_PtzManager.GetPresetSettingFromPreview();
            if (cfg != null)
            {
                PTZLogger.Log.Debug("Update every preset icons");
                PresetIcon presetIcon;
                foreach (var presetCfg in cfg)
                {
                    presetIcon = PresetIconList.Icons.FirstOrDefault(x => x.Key == presetCfg.IconKey);
                    if (presetIcon != null)
                    {
                        if (presetCfg.PresetId == "1")
                            Preset1Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "2")
                            Preset2Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "3")
                            Preset3Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "4")
                            Preset4Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "5")
                            Preset5Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "6")
                            Preset6Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "7")
                            Preset7Image = presetIcon.UriSource;
                        else if (presetCfg.PresetId == "8")
                            Preset8Image = presetIcon.UriSource;
                    }
                }
            }
        }

        private void PresetImageChangedExecute(PresetEventArgs obj)
        {

            if (obj != null)
            {
                PTZLogger.Log.Debug("Preset {0} has changed is image {1}", obj.PresetId, obj.PresetIcon.Key);
                //Update Image for current preset
                if (obj.PresetId == "1")
                    Preset1Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "2")
                    Preset2Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "3")
                    Preset3Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "4")
                    Preset4Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "5")
                    Preset5Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "6")
                    Preset6Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "7")
                    Preset7Image = obj.PresetIcon.UriSource;
                else if (obj.PresetId == "8")
                    Preset8Image = obj.PresetIcon.UriSource;
                //Save Configuration for the preview camera and the selected preset
                m_PtzManager.UpdatePresetConfiguration(obj);
            }
        }

        private void SwitcherNotification(NotificationMessage<ISwitcherParser> obj)
        {
            if (obj != null && obj.Content != null)
                if (obj.Notification == NotificationSource.SwictcherConnected)
                {
                    Switcher.Connected = obj.Content.Connected;
                }
        }

        private void GamePadNotification(NotificationMessage<IHIDParser> obj)
        {
            if (obj != null && obj.Content != null)
                if (obj.Notification == NotificationSource.GamePadConnected)
                {
                    Pad.Connected = obj.Content.Connected;
                }
        }

        //private void AtemSourceChange(NotificationMessage<AtemSourceArgs> msg)
        //{
        //    // C'est un peu le même code que dans les méthodes AtemPreviewSourceChange et AtemProgramSourceChange du PTZManager. 
        //    // Voir s'il ne faudrait pas Simplement Binder une propriété
        //    if (msg.Notification == ConstMessages.ProgramSourceChanged)
        //    {
        //        foreach (var camera in Cameras)
        //        {
        //            if (camera.Name == msg.Content.CurrentInputName)
        //                camera.SourceStatus = CameraStatusEnum.Program;
        //            else if (camera.Name == msg.Content.PreviousInputName)
        //            {
        //                if (camera.Name == m_PtzManager.CameraPreview.CameraName)
        //                    camera.SourceStatus = CameraStatusEnum.Preview;
        //                else
        //                    camera.SourceStatus = CameraStatusEnum.Off;
        //            }
        //        }
        //    }
        //    else if (msg.Notification == ConstMessages.PreviewSourceChanged)
        //    {
        //        foreach (var camera in Cameras)
        //        {
        //            if (camera.Name == msg.Content.CurrentInputName)
        //                camera.SourceStatus = CameraStatusEnum.Preview;
        //            else if (camera.Name == msg.Content.PreviousInputName)
        //            {
        //                if (camera.Name == m_PtzManager.CameraProgram.CameraName)
        //                    camera.SourceStatus = CameraStatusEnum.Program;
        //                else
        //                    camera.SourceStatus = CameraStatusEnum.Off;
        //            }
        //        }

        //    }

        //}

        private void CameraZoomStopExecute()
        {
            m_PtzManager.CameraZoomStop();
        }

        private void CameraZoomWiddeExecute()
        {
            m_PtzManager.CameraZoomWide();
        }

        private void CameraZoomTeleExecute()
        {
            m_PtzManager.CameraZoomTele();
        }

        private void CameraUpExecute()
        {
            m_PtzManager.CameraPanTiltUp();
        }
        private void CameraUpLeftExecute()
        {
            m_PtzManager.CameraPanTiltUpLeft();
        }
        private void CameraUpRightExecute()
        {
            m_PtzManager.CameraPanTiltUpRight();
        }
        private void CameraDownExecute()
        {
            m_PtzManager.CameraPanTiltDown();
        }
        private void CameraDownLeftExecute()
        {
            m_PtzManager.CameraPanTiltDownLeft();
        }
        private void CameraDownRightExecute()
        {
            m_PtzManager.CameraPanTiltDownRight();
        }
        private void CameraLeftExecute()
        {
            m_PtzManager.CameraPanTiltLeft();
        }
        private void CameraRightExecute()
        {
            m_PtzManager.CameraPanTiltRight();
        }
        private void CameraPanStopExecute()
        {
            m_PtzManager.CameraPanTiltStop();
        }
    }

    public enum PresetStatusEnum
    {
        Idle,
        WaitingForSetPreset,
        SetPreset,
        CallPreset,
        PresetSaved
    }

}
