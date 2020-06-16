using PTZPadController.DataAccessLayer;

namespace PTZPadController.BusinessLayer
{
    public interface IPTZManager
    {

        public ICameraHandler CameraPreview { get; }

        public ICameraHandler CameraProgram { get; }

        void StartUp();

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


        void AddCameraHandler(ICameraHandler camHandler);

        void SetAtemHandler(IAtemSwitcherHandler atemHandler);

        void InitSeetings(ConfigurationModel cfg);

    }
}