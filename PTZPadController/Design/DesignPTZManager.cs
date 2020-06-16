using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
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

        public void AddCameraHandler(ICameraHandler camHandler)
        {
        }

        public void CameraPanTiltUp()
        {
            //
        }

        public void CameraZoomStop()
        {
        }

        public void CameraZoomTele()
        {
        }

        public void CameraZoomWide()
        {
        }

        public void InitSeetings(ConfigurationModel cfg)
        {
        }

        public void SetAtemHandler(IAtemSwitcherHandler atemHandler)
        {
        }

        public void StartUp()
        {
        }

        void IPTZManager.CameraPanTiltDown()
        {
        }

        void IPTZManager.CameraPanTiltDownLeft()
        {
        }

        void IPTZManager.CameraPanTiltDownRight()
        {
        }

        void IPTZManager.CameraPanTiltLeft()
        {
        }

        void IPTZManager.CameraPanTiltRight()
        {
        }

        void IPTZManager.CameraPanTiltStop()
        {
        }

        void IPTZManager.CameraPanTiltUpLeft()
        {
        }

        void IPTZManager.CameraPanTiltUpRight()
        {
        }
    }
}
