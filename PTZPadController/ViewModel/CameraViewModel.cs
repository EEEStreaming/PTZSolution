using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.Common;
using PTZPadController.DataAccessLayer;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace PTZPadController.ViewModel
{
    public class CameraViewModel : ViewModelBase
    {
        #region Variables
        private string _Name = "";
        private int _InputIndex = 0;
        private bool _Connected = false;
        private ICameraParserModel _Camera;
        private CameraStatusEnum _SourceStatus;
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

        #endregion

        #region Construction/Destruction/Initialization
        public CameraViewModel(ICameraParserModel cam, int index)
        {
            _Camera = cam;
            _Name = cam.CameraName;
            _InputIndex = index;
            _SourceStatus = CameraStatusEnum.Off;
            Connected = _Camera.Connected;
            MessengerInstance.Register<NotificationMessage<ISocketParser>>(this, SocketNotification);
            MessengerInstance.Register<NotificationMessage<CameraEventArgs>>(this, CameraNotification);

            Task.Delay(500).ContinueWith((t) => 
            SourceStatus = CameraStatusEnum.Program
            );
        }

        private void CameraNotification(NotificationMessage<CameraEventArgs> obj)
        {
            if (obj.Notification == ConstMessages.CameraStatusChanged)
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