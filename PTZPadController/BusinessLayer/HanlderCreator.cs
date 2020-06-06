using PTZPadController.DataAccessLayer;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    /// <summary>
    /// Cette classe parmet de découpler le PTZManager avec les Handlers. Afin de pouvoir faire des testes unitaires du Manager.
    /// </summary>
    public class HanlderCreator : IHanlderCreator
    {
        public IAtemSwitcherHandler CreateAtemSwitcher()
        {
            return new AtemSwitcherHandler();
        }

        public ICameraHandler CreateCamera(CameraConnexionModel connexionCamera)
        {
            return new CameraPTC140Handler(connexionCamera);
        }

        public IConfigurationHandler CreateConfiguration()
        {
            return new ConfigurationFileHandler();
        }

        public IMouseHandler CreateMouse()
        {
            return new MouseHandler();
        }

        public IPadHandler CreatePad()
        {
            return new PadHandler();
        }
    }
}
