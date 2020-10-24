using System.ComponentModel;

namespace PTZPadController.DataAccessLayer
{
    public enum ECameraFocusMode
    {
        Unknown,
        Auto,
        Manual,
        OnePush
    }

    public interface ICameraParser : ICameraParserModel
    {
        void Connect();
        void Disconnect();
        void Tally(bool ledRed, bool ledGreen);

        public void PanTiltUp(short panSpeed, short tiltSpeed);
        public void PanTiltDown(short panSpeed, short tiltSpeed);
        public void PanTiltLeft(short panSpeed, short tiltSpeed);
        public void PanTiltRight(short panSpeed, short tiltSpeed);
        public void PanTiltUpLeft(short panSpeed, short tiltSpeed);
        public void PanTiltUpRight(short panSpeed, short tiltSpeed);
        public void PanTiltDownLeft(short panSpeed, short tiltSpeed);
        public void PanTiltDownRight(short panSpeed, short tiltSpeed);
        public void PanTiltStop();
        public void PanTiltHome();
        public void ZoomStop();
        public void ZoomTele();
        public void ZoomWide();
        public void ZoomTele(decimal zoomSpeed);
        public void ZoomWide(decimal zoomSpeed);
        public void CameraMemoryReset(short memory);
        public void CameraMemorySet(short memory);
        public void CameraMemoryRecall(short memory);

        // Focus
        public void FocusModeAuto();
        public void FocusModeManual();
        public void FocusModeOnePush();
        public void FocusOnePushTrigger();
        public void FocusMode();
    }

    public interface ICameraParserModel
    {
        public string CameraName { get; }
        public bool Connected { get; }
    }
}