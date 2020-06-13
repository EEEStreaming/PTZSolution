using PTZPadController.DataAccessLayer;

namespace PTZPadController.BusinessLayer
{
    public interface ICameraHandler
    {
        public string CameraName { get;  }

        public bool PanTileWorking { get; }
        public bool ZoomWorking { get; }
        void Initialize(ICameraParser camParser, string name);
        void PanTiltUp();
        void Tally(bool ledRed, bool ledGreen);
        void Connect();
        void StopPanTile();
    }
}