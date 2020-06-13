namespace PTZPadController.DataAccessLayer
{
    public interface ICameraParser
    {

        void Connect();
        void Disconnect();
        void Tally(bool ledRed, bool ledGreen);
    }
}