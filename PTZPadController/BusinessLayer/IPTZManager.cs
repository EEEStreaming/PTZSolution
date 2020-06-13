namespace PTZPadController.BusinessLayer
{
    public interface IPTZManager
    {

        public ICameraHandler CameraPreview { get; }

        public ICameraHandler CameraProgram { get; }

        void StartUp();

        void CameraPanTiltUp();

        void AddCcameraHandler(ICameraHandler camHandler);

        void SetAtemHandler(IAtemSwitcherHandler atemHandler);
    }
}