namespace PTZPadController.DataAccessLayer
{
    public interface ISocketParser
    {
        bool Connected { get;}

        public void Initialize(string name, string host, int port, IClientCallback callback);

        void Connect();
        void Disconnect();

        void SendData(byte[] msg);

    }
}