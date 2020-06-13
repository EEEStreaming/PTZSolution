using PTZPadController.DataAccessLayer;

namespace PTZPadController.BusinessLayer
{
    public interface ICameraHandler
    {
        void Initialize(ICameraParser camParser);
        void PanTiltUp();
        void Tally(bool ledRed, bool ledGreen);
    }
}