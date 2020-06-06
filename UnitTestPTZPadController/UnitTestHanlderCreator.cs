using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestPTZPadController
{
    class UnitTestHanlderCreator : IHanlderCreator
    {
        public IAtemSwitcherHandler CreateAtemSwitcher()
        {
            throw new NotImplementedException();
        }

        public ICameraHandler CreateCamera(CameraConnexionModel connexionCamera)
        {
            throw new NotImplementedException();
        }

        public IConfigurationHandler CreateConfiguration()
        {
            throw new NotImplementedException();
        }

        public IMouseHandler CreateMouse()
        {
            throw new NotImplementedException();
        }

        public IPadHandler CreatePad()
        {
            throw new NotImplementedException();
        }
    }
}
