using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System.Collections.Generic;
using System.Windows;
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
        void CameraPanTiltUp(short moveSpeed);
        void CameraPanTiltUpLeft(short moveSpeed);
        void CameraPanTiltUpRight(short moveSpeed);
        void CameraPanTiltDown(short moveSpeed);
        void CameraPanTiltDownLeft(short moveSpeed);
        void CameraPanTiltDownRight(short moveSpeed);
        void CameraPanTiltLeft(short moveSpeed);
        void CameraPanTiltRight(short moveSpeed);
        void CameraPanTiltStop();
        void CameraZoomStop();
        void CameraZoomWide();
        void CameraZoomTele();
        void CameraZoomWide(short zoomSpeed);
        void CameraZoomTele(short zoomSpeed);
        void CameraButtonPresetDown(int preset);
        void CameraButtonPresetUp(int preset);

        // Focus
        public void CameraFocusModeAuto();
        void SaveWindowPosition(Window window, WindowPositionModel winPos);

        WindowPositionModel LoadWindowPosition(Window window);
        public void CameraFocusModeManual();
        public void CameraFocusModeOnePush();
        public void CameraFocusOnePushTrigger();


        void AddCameraHandler(ICameraHandler camHandler);

        void SetSwitcherHandler(ISwitcherHandler atemHandler);

        void InitSeetings(ConfigurationModel cfg);
        void SendSwitcherTransition(TransitionEnum transition);
        void SetSwitcherPreview(string cameraName);
        void NextSwitcherPreview();

        void UpdatePresetConfiguration(PresetEventArgs obj);
        List<PresetIconSettingModel> GetPresetSettingFromPreview();
        void AddGamePad(IGamePadHandler pad);
        void CameraFocusAutoOnePushSwitchMode();
        ECameraFocusMode GetCameraFocusMode(string name);
    }
}