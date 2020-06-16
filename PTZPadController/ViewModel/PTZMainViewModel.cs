using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.Common;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.ObjectModel;
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
            Cameras = new ObservableCollection<CameraViewModel>();

            //Initialize viewModel
            CameraViewModel camvm;
            int cameraInput = 1;
            
            foreach (var cam in m_PtzManager.Cameras)
            {
                camvm = new CameraViewModel(cam.Parser, cameraInput);
                Cameras.Add(camvm);
                cameraInput++;
            }

            Messenger.Default.Register<NotificationMessage<AtemSourceArgs>>(this, AtemSourceChange);

        }

        private void AtemSourceChange(NotificationMessage<AtemSourceArgs> msg)
        {
            if (msg.Notification == ConstMessages.ProgramSourceChanged)
            {
                foreach (var camera in Cameras)
                {
                    if (camera.Name == msg.Content.CurrentInputName)
                        camera.SourceStatus = SourceEnum.Program;
                    else if (camera.Name == msg.Content.PreviousInputName)
                    {
                        if (camera.Name == m_PtzManager.CameraPreview.CameraName)
                            camera.SourceStatus = SourceEnum.Preview;
                        else
                            camera.SourceStatus = SourceEnum.Off;
                    }
                }
            }
            else if (msg.Notification == ConstMessages.PreviewSourceChanged)
            {
                foreach (var camera in Cameras)
                {
                    if (camera.Name == msg.Content.CurrentInputName)
                        camera.SourceStatus = SourceEnum.Preview;
                    else if (camera.Name == msg.Content.PreviousInputName)
                    {
                        if (camera.Name == m_PtzManager.CameraProgram.CameraName)
                            camera.SourceStatus = SourceEnum.Program;
                        else
                            camera.SourceStatus = SourceEnum.Off;
                    }
                }

            }

        }

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

namespace PTZPadController
{
    enum SourceEnum
    {
        Program,
        Off,
        Preview
    }
}