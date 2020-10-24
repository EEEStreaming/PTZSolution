using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.BusinessLayer
{
    // Define 6 regions in the axis of the gamepads controllers
    

    public class GamePadHandler : IGamePadHandler
    {
        private IHIDParser m_HidParser;
        private IPTZManager m_PtzManager;

        // State machines
        private JoystickStateMachine m_ZoomState;
        private JoystickStateMachine m_PanState;
        private AtemStateMachine m_AtemState;
        private PresetStateMachine m_PresetState;
        private FocusStateMachine m_FocusState;

        public void Camera1SetPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down && m_PtzManager.Cameras.Count >= 1)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[0].CameraName));
            }
        }

        public void Camera2SetPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down && m_PtzManager.Cameras.Count >= 2)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[1].CameraName));
            }
        }

        public void Camera3SetPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down && m_PtzManager.Cameras.Count >= 3)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[2].CameraName));
            }
        }

        public void Camera4SetPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down && m_PtzManager.Cameras.Count >= 4)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[3].CameraName));
            }
        }

        public void CameraFocusAutoMode(ButtonCommand button)
        {
        }

        public void CameraFocusAutoOnePushSwitchMode(ButtonCommand button)
        {
        }

        public void CameraFocusOnePushMode(ButtonCommand button)
        {
        }

        public void CameraFocusOnePushTriger(ButtonCommand button)
        {
        }

        public void CameraPanTiltAxes(double x, double y)
        {
            if (m_PanState.CurrentState != m_PanState.MoveNext(x, y))
            {
                switch(m_PanState.CurrentState)
                {
                    case (AxisPosition.CENTER, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltStop();
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.CENTER):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.CENTER):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltLeft();
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.CENTER):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.CENTER):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltRight();
                        break;
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDown();
                        break;
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUp();
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDownLeft();
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUpLeft();
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDownRight();
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUpRight();
                        break;

                }
            }

        }

        public void CameraPreset1(ButtonCommand button)
        {
            ExecutePresetButton(1, button);
        }

        public void CameraPreset2(ButtonCommand button)
        {
            ExecutePresetButton(2, button);
        }

        public void CameraPreset3(ButtonCommand button)
        {
            ExecutePresetButton(3, button);
        }

        public void CameraPreset4(ButtonCommand button)
        {
            ExecutePresetButton(4, button);
        }

        public void CameraPreset5(ButtonCommand button)
        {
            ExecutePresetButton(5, button);
        }

        public void CameraPreset6(ButtonCommand button)
        {
            ExecutePresetButton(6, button);
        }

        public void CameraPreset7(ButtonCommand button)
        {
            ExecutePresetButton(7, button);
        }

        public void CameraPreset8(ButtonCommand button)
        {
            ExecutePresetButton(8, button);
        }

        private void ExecutePresetButton(int preset, ButtonCommand command)
        {
            switch (command)
            {
                case ButtonCommand.Down:
                    m_PtzManager.CameraButtonPresetDown(preset);
                    break;
                case ButtonCommand.Up:
                    m_PtzManager.CameraButtonPresetUp(preset);
                    break;
            }
        }

        public void CameraZoomAxe(double y)
        {
            if (m_ZoomState.CurrentState != m_ZoomState.MoveNext(0.5, y))
            {
                switch (m_ZoomState.CurrentState.Item2)
                {
                    case AxisPosition.CENTER:
                        m_PtzManager.CameraZoomStop();
                        break;
                    case AxisPosition.POSITIVE_CLOSE:
                    case AxisPosition.POSITIVE_MEDIUM:
                    case AxisPosition.POSITIVE_FAR:
                        m_PtzManager.CameraZoomTele();
                        break;
                    case AxisPosition.NEGATIVE_CLOSE:
                    case AxisPosition.NEGATIVE_MEDIUM:
                    case AxisPosition.NEGATIVE_FAR:
                        m_PtzManager.CameraZoomWide();
                        break;
                }
            }
        }

        public void ConnectTo()
        {
            m_HidParser.ExecuteAsync().ConfigureAwait(true);
        }

        public void Disconnect()
        {
            m_HidParser.StopAsync();
        }

        public void Initialize(IHIDParser hidParser, IPTZManager manager)
        {
            m_HidParser = hidParser;
            m_PtzManager = manager;
            m_ZoomState = new JoystickStateMachine();
            m_PanState = new JoystickStateMachine();
        }

        public void SwitcherCut(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SendSwitcherTransition(Messages.TransitionEnum.Cut));
            }

        }

        public void SwitcherMix(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SendSwitcherTransition(Messages.TransitionEnum.Mix));
            }
        }
    }
}
