
namespace PTZPadController.BusinessLayer
{
    public interface IGamePadHandler
    {
        void Initialize(IHIDParser hidParser, IPTZManager manager);
        void ConnectTo();
        void Disconnect();

        void CameraPanTiltUp();
        void CameraPanTiltUpLeft();
        void CameraPanTiltUpRight();
        void CameraPanTiltDown();
        void CameraPanTiltDownLeft();
        void CameraPanTiltDownRight();
        void CameraPanTiltLeft();
        void CameraPanTiltRight();
        void CameraPanTiltStop();
        void CameraZoomStop();
        void CameraZoomWide();
        void CameraZoomTele();
        void CameraCallPreset(int preset);
        void CameraSetPreset(int preset);
        void CameraSetPreview(int cameraid);
    }
}