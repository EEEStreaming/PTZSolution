
using System.Collections.Generic;

namespace PTZPadController.BusinessLayer
{
    public interface IGamePadHandler
    {
        void Initialize(IHIDParser hidParser, IPTZManager manager, Dictionary<string, short> speedConfig);
        void ConnectTo();
        void Disconnect();

        void CameraPanTiltAxes(double x, double y);
      
        void CameraZoomAxe(double x);

        void CameraPreset1(ButtonCommand button);
        void CameraPreset2(ButtonCommand button);
        void CameraPreset3(ButtonCommand button);
        void CameraPreset4(ButtonCommand button);
        void CameraPreset5(ButtonCommand button);
        void CameraPreset6(ButtonCommand button);
        void CameraPreset7(ButtonCommand button);
        void CameraPreset8(ButtonCommand button);
        void CameraFocusAutoMode(ButtonCommand button);
        void CameraFocusOnePushMode(ButtonCommand button);
        void CameraFocusOnePushTriger(ButtonCommand button);
        void CameraFocusAutoOnePushSwitchMode(ButtonCommand button);
        void Camera1SetPreview(ButtonCommand button);
        void Camera2SetPreview(ButtonCommand button);
        void Camera3SetPreview(ButtonCommand button);
        void Camera4SetPreview(ButtonCommand button);
        void CameraNextPreview(ButtonCommand button);
        public void CameraProgramSetPreview(ButtonCommand button);

        void SwitcherCut(ButtonCommand button);
        void SwitcherMix(ButtonCommand button);
    }

    public enum ButtonCommand
    {
        Up,
        Down,
        NA
    }
}