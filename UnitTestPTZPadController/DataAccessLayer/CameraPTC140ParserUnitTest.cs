using NSubstitute;
using NUnit.Framework;
using PTZPadController.DataAccessLayer;

namespace UnitTestPTZPadController.DataAccessLayer
{
    public class CameraPTC140ParserUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTally()
        {
            ISocketParser socket = new MockISocketParser(); //Substitute.For<ISocketParser>();

            CameraPTC140Parser camera = new CameraPTC140Parser();
            camera.Initialize(socket);

            camera.Connect();

            (socket as MockISocketParser).SetTestValue(0x02, 0x03);
            camera.Tally(true, false);

            (socket as MockISocketParser).SetTestValue(0x03, 0x02);
            camera.Tally(false, true);

            (socket as MockISocketParser).SetTestValue(0x03, 0x03);
            camera.Tally(false, false);

            (socket as MockISocketParser).SetTestValue(0x02, 0x03);
            camera.Tally(true, true);
        }

        [Test]
        public void TestConvertSpeed()
        {
            //Assert. ConvertSpeed(0, 0);

        }
    }

    public class MockISocketParser : ISocketParser
    {
        public bool Connected { get; set; }

        public string SocketName { get; set; }

        private byte ledRed;
        private byte ledGreen;


        public void Connect()
        {
            Connected = true;
        }

        public void Disconnect()
        {
            Connected = false;
        }

        public void Initialize(string name, string host, int port, IClientCallback callback)
        {

        }

        public void SetTestValue(byte ledRed, byte ledGreen)
        {
            this.ledRed = ledRed;
            this.ledGreen = ledGreen;
        }

        public void SendData(byte[] msg)
        {
            Assert.IsTrue(msg[8] == ledRed);
            Assert.IsTrue(msg[9] == ledGreen);
        }
    }
}