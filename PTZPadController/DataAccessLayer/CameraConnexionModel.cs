using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{
    public class CameraConnexionModel
    {
        public string CameraName { get; set; }
        public string CameraHost { get; set; }
        public int CameraPort { get; set; }

        public List<PresetIconSettingModel> PresetIcons { get; set; }

        public CameraConnexionModel()
        {
            PresetIcons = new List<PresetIconSettingModel>();
        }
    }
}
