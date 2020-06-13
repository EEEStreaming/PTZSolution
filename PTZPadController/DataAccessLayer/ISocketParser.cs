namespace PTZPadController.DataAccessLayer
{
    public interface ISocketParser
    {
        public void Initialize(string name, string host, int port, IClientCallback callback);

        void Connect();
        void Disconnect();
    }
}