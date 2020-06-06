using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{
    public class CameraConnexionModel
    {
        public string CameraName { get; internal set; }
        public string CameraHost { get; internal set; }
        public int CameraPort { get; internal set; }
    }
}
