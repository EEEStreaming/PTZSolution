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

        public void CameraCallPreset(int preset)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUp()
        {
            //
        }

        public void CameraSetPreset(int preset)
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
    }
}
