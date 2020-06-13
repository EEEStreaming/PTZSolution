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

        public string AtemHost { get; set; }

        public List<CameraConnexionModel> Cameras = new List<CameraConnexionModel>();
    }
}
