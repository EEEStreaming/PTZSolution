using PTZPadController.DataAccessLayer;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    public interface ICameraHandler
    {
        public ICameraParserModel Parser { get; }
        public string CameraName { get;  }

        public bool PanTileWorking { get; }
        public bool ZoomWorking { get; }
        void Initialize(ICameraParser camParser);
        void Tally(bool ledRed, bool ledGreen);
        Task ConnectTo();
        /// <summary>
        /// If a connection is in progress, waits it's completition and then returns the switcher connection status.
        /// </summary>
        /// <returns></returns>
        bool WaitForConnection();

        public void PanTiltUp(short panSpeed, short tiltSpeed);
        public void PanTiltDown(short panSpeed, short tiltSpeed);
        public void PanTiltLeft(short panSpeed, short tiltSpeed);
        public void PanTiltRight(short panSpeed, short tiltSpeed);
        public void PanTiltUpLeft(short panSpeed, short tiltSpeed);
        public void PanTiltUpRight(short panSpeed, short tiltSpeed);
        public void PanTiltDownLeft(short panSpeed, short tiltSpeed);
        public void PanTiltDownRight(short panSpeed, short tiltSpeed);
        public void PanTiltStop();
        public void ZoomStop();
        public void ZoomTele();
        public void ZoomWide();
        public void ZoomTele(decimal zoomSpeed);
        public void ZoomWide(decimal zoomSpeed);
        public void PanTiltHome();
        public void CameraMemoryReset(short memory);
        public void CameraMemorySet(short memory);
        public void CameraMemoryRecall(short memory);

        //Focus
        public void FocusModeAuto(); 
        public void FocusModeManual();
        public void FocusModeOnePush();
        public void FocusOnePushTrigger();
        public void GetFocusMode();

    }
}