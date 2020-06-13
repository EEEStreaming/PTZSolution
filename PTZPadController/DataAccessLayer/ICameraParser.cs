namespace PTZPadController.DataAccessLayer
{
    public interface ICameraParser
    {

        bool Connected { get;}
        void Connect();
        void Disconnect();
        void Tally(bool ledRed, bool ledGreen);
    }
}