using PTZPadController.DataAccessLayer;

namespace PTZPadController.BusinessLayer
{
    public interface ICameraHandler
    {
        public string CameraName { get;  }

        public bool PanTileWorking { get; }
        public bool ZoomWorking { get; }
        void Initialize(ICameraParser camParser, string name);
        void Tally(bool ledRed, bool ledGreen);
        void Connect();

        public void PanTiltUp(short panSpeed, short tiltSpeed);
        public void PanTiltDown(short panSpeed, short tiltSpeed);
        public void PanTiltLeft(short panSpeed, short tiltSpeed);
        public void PanTiltRight(short panSpeed, short tiltSpeed);
        public void PanTiltUpLeft(short panSpeed, short tiltSpeed);
        public void PanTiltUpRight(short panSpeed, short tiltSpeed);
        public void PanTiltDownLeft(short panSpeed, short tiltSpeed);
        public void PanTiltDownRight(short panSpeed, short tiltSpeed);
        public void PanTiltStop();
        public void ZoomTele();
        public void ZoomWide();
        public void ZoomStop();
    }
}