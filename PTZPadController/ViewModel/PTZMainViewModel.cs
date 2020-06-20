using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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
        private string _SwitcherName;
        private bool _SwitcherConnected;
        private DeviceItemViewModel _Switcher;
        private DeviceItemViewModel _Pad;

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
        public ICommand SwitcherCallPreset { get; private set; }
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
            SwitcherCallPreset = new RelayCommand<string>(SwitcherCallPresetExecute);
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


            //Startup the whole system 
            //Multi-thread get issue with ATEM SDK.
            m_PtzManager.StartUp();
            //Task.Factory.StartNew(()=>ptzManager.StartUp());

        }

        private void SwitcherCallPresetExecute(string preset)
        {
            int iPreset;
            if (Int32.TryParse(preset, out  iPreset))
                m_PtzManager.CameraCallPreset(iPreset);
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
                var camera = Cameras.FirstOrDefault(x => x.Name == obj.Content.CameraName);
                if (camera != null)
                    camera.SourceStatus = obj.Content.Status;
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
}