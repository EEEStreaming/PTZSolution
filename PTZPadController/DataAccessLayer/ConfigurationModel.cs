using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{

    public class ConfigurationModel
    {
        public bool UseTallyGreen { get; set; }
        public string AtemHost { get; set; }

        public List<CameraConnexionModel> Cameras = new List<CameraConnexionModel>();
        public List<HIDGamePadModel> GamePads = new List<HIDGamePadModel>();
        public Dictionary<string, short> CamSpeed = new Dictionary<string, short>();

        public Dictionary<string, WindowPositionModel> WindowsPosition = new Dictionary<string, WindowPositionModel>();

    }
}
