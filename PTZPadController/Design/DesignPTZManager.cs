using PTZPadController.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.Design
{
    public class DesignPTZManager : IPTZManager
    {
        public void AddCcameraHandler(ICameraHandler camHandler)
        {
        }

        public void CameraPanTiltUp()
        {
            //
        }

        public void SetAtemHandler(IAtemSwitcherHandler atemHandler)
        {
            throw new NotImplementedException();
        }

        public void StartUp()
        {
        }
    }
}
