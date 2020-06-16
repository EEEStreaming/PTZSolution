using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using System;
using System.Dynamic;

namespace PTZPadController.ViewModel
{
    public class CameraViewModel : ViewModelBase
    {
        #region Variables
        private string _Name = "";
        private int _InputIndex = 0;
        private bool _Connected = false;
        private ICameraParserModel _Camera;
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

        internal SourceEnum SourceStatus { get; set; }

        #endregion

        #region Construction/Destruction/Initialization
        public CameraViewModel(ICameraParserModel cam, int index)
        {
            _Camera = cam;
            _Name = cam.CameraName;
            _InputIndex = index;
            Connected = _Camera.Connected;
            MessengerInstance.Register<NotificationMessage<ISocketParser>>(this, SocketNotification);
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