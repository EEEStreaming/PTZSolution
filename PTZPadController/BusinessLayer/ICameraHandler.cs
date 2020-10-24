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

        public void PanTiltUp(short moveSpeed);
        public void PanTiltDown(short moveSpeed);
        public void PanTiltLeft(short moveSpeed);
        public void PanTiltRight(short moveSpeed);
        public void PanTiltUpLeft(short moveSpeed);
        public void PanTiltUpRight(short moveSpeed);
        public void PanTiltDownLeft(short moveSpeed);
        public void PanTiltDownRight(short moveSpeed);
        public void PanTiltStop();
        public void ZoomStop();
        public void ZoomTele();
        public void ZoomWide();
        public void ZoomTele(short zoomSpeed);
        public void ZoomWide(short zoomSpeed);
        public void PanTiltHome();
        public void CameraMemoryReset(short memory);
        public void CameraMemorySet(short memory);
        public void CameraMemoryRecall(short memory);

        //Focus
        public void FocusModeAuto(); 
        public void FocusModeManual();
        public void FocusModeOnePush();
        public void FocusOnePushTrigger();

    }
}