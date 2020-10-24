using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
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

        public List<ICameraHandler> Cameras { get; set; }

        public void AddCameraHandler(ICameraHandler camHandler)
        {
        }

        public void AddGamePad(IGamePadHandler pad)
        {
            throw new NotImplementedException();
        }

        public void CameraButtonPresetDown(int preset)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUp()
        {
            //
        }

        public void CameraButtonPresetUp(int preset)
        {
            throw new NotImplementedException();
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

        public void CameraZoomTele(short zoomSpeed)
        {
        }

        public void CameraZoomWide(short zoomSpeed)
        {
        }

        public List<PresetIconSettingModel> GetPresetSettingFromPreview()
        {
            throw new NotImplementedException();
        }

        public void InitSeetings(ConfigurationModel cfg)
        {
        }

        public void SendSwitcherTransition(TransitionEnum transition)
        {
        }

        public void SetSwitcherHandler(ISwitcherHandler atemHandler)
        {
        }

        public void SetSwitcherPreview(string cameraName)
        {
        }

        public void StartUp()
        {
        }

        public void UpdatePresetConfiguration(PresetEventArgs obj)
        {
            throw new NotImplementedException();
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

        public void CameraFocusModeAuto()
        {
        }

        public void CameraFocusModeManual()
        {
        }

        public void CameraFocusModeOnePush()
        {
        }

        public void CameraFocusOnePushTrigger()
        {
        }

        public void CameraGetFocusMode()
        {
            throw new NotImplementedException();
        }
    }
}
