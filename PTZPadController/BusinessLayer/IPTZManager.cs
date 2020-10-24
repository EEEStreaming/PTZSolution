using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System.Collections.Generic;
using System.Windows.Documents;

namespace PTZPadController.BusinessLayer
{
    public interface IPTZManager
    {

        public ICameraHandler CameraPreview { get; }

        public ICameraHandler CameraProgram { get; }

        public System.Collections.Generic.List<ICameraHandler> Cameras { get; }

        void StartUp();

        void CameraPanTiltUp();
        void CameraPanTiltUpLeft();
        void CameraPanTiltUpRight();
        void CameraPanTiltDown();
        void CameraPanTiltDownLeft();
        void CameraPanTiltDownRight();
        void CameraPanTiltLeft();
        void CameraPanTiltRight();
        void CameraPanTiltStop();
        void CameraZoomStop();
        void CameraZoomWide();
        void CameraZoomTele();
        void CameraZoomWide(short zoomSpeed);
        void CameraZoomTele(short zoomSpeed);
        void CameraCallPreset(int preset);
        void CameraSetPreset(int preset);

        // Focus
        public void CameraFocusModeAuto();
        public void CameraFocusModeManual();
        public void CameraFocusModeOnePush();
        public void CameraFocusOnePushTrigger();

        public void CameraGetFocusMode();

        void AddCameraHandler(ICameraHandler camHandler);

        void SetSwitcherHandler(ISwitcherHandler atemHandler);

        void InitSeetings(ConfigurationModel cfg);
        void SendSwitcherTransition(TransitionEnum transition);
        void SetSwitcherPreview(string cameraName);
        void UpdatePresetConfiguration(PresetEventArgs obj);
        List<PresetIconSettingModel> GetPresetSettingFromPreview();
    }
}