﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PTZPadController.ViewModel
{
    public class CameraViewModel : ViewModelBase
    {
        #region Variables
        private string _Name = "";
        private int _InputIndex = 0;
        private bool _Connected = false;
        private IPTZManager _Manager;
        private CameraStatusEnum _SourceStatus;
        #endregion

        #region Commands
        public ICommand AtemPreview { get; private set; }
        #endregion
        
        #region Properties
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public int InputIndex
        {
            get { return _InputIndex; }
            set
            {
                if (_InputIndex == value) return;
                _InputIndex = value;
                RaisePropertyChanged("InputIndex");
            }
        }

        public bool Connected
        {
            get { return _Connected; }
            set
            {
                if (_Connected == value) return;
                _Connected = value;
                RaisePropertyChanged("Connected");
            }
        }

        public CameraStatusEnum SourceStatus {
            get { return _SourceStatus; }
            set
            {
                if (_SourceStatus == value) return;
                _SourceStatus = value;
                RaisePropertyChanged("SourceStatus");
            }
        }

        public ECameraFocusMode FocusMode { get; internal set; }

        #endregion

        #region Construction/Destruction/Initialization
        public CameraViewModel(IPTZManager manager, ICameraParserModel cam, int index)
        {
            _Manager = manager;
            _Name = cam.CameraName;
            _InputIndex = index;
            _SourceStatus = CameraStatusEnum.Off;
            Connected = cam.Connected;
            AtemPreview = new RelayCommand(AtemSetPreviewExecute);
            MessengerInstance.Register<NotificationMessage<ISocketParser>>(this, SocketNotification);
            MessengerInstance.Register<NotificationMessage<CameraStatusMessageArgs>>(this, CameraNotification);
        }

        private void AtemSetPreviewExecute()
        {
            _Manager.SetSwitcherPreview(_Name);
        }

        private void CameraNotification(NotificationMessage<CameraStatusMessageArgs> obj)
        {
            if (obj.Notification == NotificationSource.CameraStatusChanged)
            {
                if (obj.Content.CameraName == _Name)
                {
                    SourceStatus = obj.Content.Status;
                }
            }
        }

        private void SocketNotification(NotificationMessage<ISocketParser> msg)
        {
            if (msg.Content.SocketName == _Name)
            {
                //It's my socket
                Connected = msg.Content.Connected;
            }
        }

        #endregion

        #region Methods

        #endregion

    }
}