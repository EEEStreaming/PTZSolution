namespace PTZPadController.BusinessLayer
{
    public interface IPTZManager
    {

        void StartUp();

        void CameraPanTiltUp();

        void AddCcameraHandler(ICameraHandler camHandler);
    }
}