namespace PTZPadController.DataAccessLayer
{
    public interface ICameraParser
    {

        bool Connected { get;}
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

        public void ZoomTele();
        public void ZoomWide();
        public void ZoomStop();

    }
}