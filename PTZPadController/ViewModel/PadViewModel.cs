using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.ViewModel
{
    public class PadViewModel : ViewModelBase
    {
        private string _Name;
        private bool _Connected;
        private PadStatusEnum _Status;
        private bool _InverseAxeY;
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

        public PadStatusEnum Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;
                _Status = value;
                RaisePropertyChanged("Status");
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
                UpdateStatus();
            }
        }



        public bool InverseAxeY
        {
            get { return _InverseAxeY; }
            set
            {
                if (_InverseAxeY == value) return;
                _InverseAxeY = value;
                RaisePropertyChanged("InverseAxeY");
                UpdateStatus();
            }
        }
        #endregion

        #region Construction/Destruction/Initialization
        public PadViewModel(string name)
        {
            Name = name;
            Connected = false;
            Status = PadStatusEnum.DisconnectedUp;
        }
        #endregion

        private void UpdateStatus()
        {
            if (_Connected && _InverseAxeY)
                Status = PadStatusEnum.ConnectedDown;
            else if (_Connected && !_InverseAxeY)
                Status = PadStatusEnum.ConnectedUp;
            else if (!_Connected && _InverseAxeY)
                Status = PadStatusEnum.DisconnectedDown;
            else if (!_Connected && !_InverseAxeY)
                Status = PadStatusEnum.DisconnectedUp;
        }
    }

    public enum PadStatusEnum
    {
        DisconnectedUp,
        DisconnectedDown,
        ConnectedUp,
        ConnectedDown,
    }
}
