using NSubstitute;
using NUnit.Framework;
using PTZPadController.DataAccessLayer;

namespace UnitTestPTZPadController
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
            ISocketParser socket = Substitute.For<ISocketParser>();

            CameraPTC140Parser camera = new CameraPTC140Parser();
            camera.Initialize(socket);

            camera.Tally(true, false);
            socket.Received().SendData(new byte[] { 0x00, 0x0B, 0x81, 0x01, 0x7E, 0x01, 0x0A, 0x00, 0x02, 0x03, 0xFF });
        }
    }
}