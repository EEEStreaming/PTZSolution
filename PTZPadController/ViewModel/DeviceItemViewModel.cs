using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.ViewModel
{
    public class DeviceItemViewModel : ViewModelBase
    {
        private string _Name;
        private bool _Connected;
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
        #endregion

        #region Construction/Destruction/Initialization
        public DeviceItemViewModel(string name)
        {
            Name = name;
            Connected = false;
        }
        #endregion


    }
}
