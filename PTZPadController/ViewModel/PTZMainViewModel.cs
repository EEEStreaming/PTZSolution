using GalaSoft.MvvmLight;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;

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
        private CameraConnexionModel m_ConnexionCamera1;
        private CameraPTC140Handler m_Camera1;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public PTZMainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            ///
            m_ConnexionCamera1 = new CameraConnexionModel { CameraName = "CAM 1", CameraHost = "127.0.0.1", CameraPort = 5002 };
            m_Camera1 = new CameraPTC140Handler(m_ConnexionCamera1);
            m_Camera1.Initialize();
            m_Camera1.PanTiltUp();
        }
    }
}