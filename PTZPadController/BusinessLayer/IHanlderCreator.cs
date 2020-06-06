using PTZPadController.DataAccessLayer;

namespace PTZPadController.BusinessLayer
{
    public interface IHanlderCreator
    {
        ICameraHandler CreateCamera(CameraConnexionModel connexionCamera);
        IAtemSwitcherHandler CreateAtemSwitcher();
        IConfigurationHandler CreateConfiguration();
        IMouseHandler CreateMouse();
        IPadHandler CreatePad();
    }
}