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

        public void PanTiltUp(short moveSpeed);
        public void PanTiltDown(short moveSpeed);
        public void PanTiltLeft(short moveSpeed);
        public void PanTiltRight(short moveSpeed);
        public void PanTiltUpLeft(short moveSpeed);
        public void PanTiltUpRight(short moveSpeed);
        public void PanTiltDownLeft(short moveSpeed);
        public void PanTiltDownRight(short moveSpeed);
        public void PanTiltStop();
        public void PanTiltHome();
        public void ZoomStop();
        public void ZoomTele();
        public void ZoomWide();
        public void ZoomTele(short zoomSpeed);
        public void ZoomWide(short zoomSpeed);
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