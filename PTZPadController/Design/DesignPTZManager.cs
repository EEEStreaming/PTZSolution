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
        public ICameraHandler CameraPreview { get; set; }

        public ICameraHandler CameraProgram { get; set; }

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

        void IPTZManager.CameraPanTiltDown()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltDownLeft()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltDownRight()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltLeft()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltRight()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltStop()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltUpLeft()
        {
            throw new NotImplementedException();
        }

        void IPTZManager.CameraPanTiltUpRight()
        {
            throw new NotImplementedException();
        }
    }
}
