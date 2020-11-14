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
        public const short SPEED_MEDIUM = 6;

        public ICameraHandler CameraPreview { get; }

        public ICameraHandler CameraProgram { get; }

        public short CameraSensitivity { get; set; }

        public System.Collections.Generic.List<ICameraHandler> Cameras { get; }

        void StartUp();

        void CameraPanTiltUp(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltUpLeft(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltUpRight(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltDown(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltDownLeft(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltDownRight(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltLeft(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltRight(short moveSpeed = SPEED_MEDIUM);
        void CameraPanTiltStop();
        void CameraZoomStop();
        void CameraZoomWide(short zoomSpeed = SPEED_MEDIUM);
        void CameraZoomTele(short zoomSpeed = SPEED_MEDIUM);
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