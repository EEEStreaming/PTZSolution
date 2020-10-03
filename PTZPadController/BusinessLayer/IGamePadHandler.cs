
namespace PTZPadController.BusinessLayer
{
    public interface IGamePadHandler
    {
        void Initialize(IHIDParser hidParser, IPTZManager manager);
        void ConnectTo();
        void Disconnect();
    }
}