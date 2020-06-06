using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using System;
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
        public ICommand CameraDown { get; private set; }
        public ICommand CameraLeft { get; private set; }
        public ICommand CameraRight { get; private set; }
        #endregion


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

            
        }

        private void CameraUpExecute()
        {
            m_PtzManager.CameraPanTiltUp();
        }
    }
}