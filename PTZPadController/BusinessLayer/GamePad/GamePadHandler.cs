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
        private Dictionary<string, short> m_CamSpeed;

        // State machines
        private JoystickStateMachine m_ZoomState;
        private JoystickStateMachine m_PanState;

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

        public void CameraProgramSetPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.CameraProgram.CameraName));
            }
        }

        public void CameraNextPreview(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.NextSwitcherPreview());
            }

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



        public void CameraFocusAutoMode(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
                m_PtzManager.CameraFocusModeAuto();
        }

        public void CameraFocusAutoOnePushSwitchMode(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
                m_PtzManager.CameraFocusAutoOnePushSwitchMode();
        }

        public void CameraFocusOnePushMode(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
                m_PtzManager.CameraFocusModeOnePush();
        }

        public void CameraFocusOnePushTriger(ButtonCommand button)
        {
            if (button == ButtonCommand.Down)
                m_PtzManager.CameraFocusOnePushTrigger();
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
                        m_PtzManager.CameraPanTiltLeft(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltLeft(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltLeft(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltRight(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltRight(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.CENTER):
                        m_PtzManager.CameraPanTiltRight(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_CLOSE):
                        m_PtzManager.CameraPanTiltDown(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltDown(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDown(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_CLOSE):
                        m_PtzManager.CameraPanTiltUp(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltUp(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.CENTER, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUp(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_CLOSE):
                        m_PtzManager.CameraPanTiltDownLeft(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltDownLeft(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDownLeft(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_CLOSE):
                        m_PtzManager.CameraPanTiltUpLeft(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltUpLeft(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.NEGATIVE_CLOSE, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.NEGATIVE_MEDIUM, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.NEGATIVE_FAR, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUpLeft(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_CLOSE):
                        m_PtzManager.CameraPanTiltDownRight(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltDownRight(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.NEGATIVE_FAR):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_CLOSE):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.NEGATIVE_FAR):
                        m_PtzManager.CameraPanTiltDownRight(m_CamSpeed["PanSpeedHigh"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_CLOSE):
                        m_PtzManager.CameraPanTiltUpRight(m_CamSpeed["PanSpeedLow"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.POSITIVE_MEDIUM):
                        m_PtzManager.CameraPanTiltUpRight(m_CamSpeed["PanSpeedMedium"]);
                        break;
                    case (AxisPosition.POSITIVE_CLOSE, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.POSITIVE_MEDIUM, AxisPosition.POSITIVE_FAR):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_CLOSE):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_MEDIUM):
                    case (AxisPosition.POSITIVE_FAR, AxisPosition.POSITIVE_FAR):
                        m_PtzManager.CameraPanTiltUpRight(m_CamSpeed["PanSpeedHigh"]);
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
                        m_PtzManager.CameraZoomTele(m_CamSpeed["ZoomSpeedLow"]);
                        break;
                    case AxisPosition.POSITIVE_MEDIUM:
                        m_PtzManager.CameraZoomTele(m_CamSpeed["ZoomSpeedMedium"]);
                        break;
                    case AxisPosition.POSITIVE_FAR:
                        m_PtzManager.CameraZoomTele(m_CamSpeed["ZoomSpeedHigh"]);
                        break;
                    case AxisPosition.NEGATIVE_CLOSE:
                        m_PtzManager.CameraZoomWide(m_CamSpeed["ZoomSpeedLow"]);
                        break;
                    case AxisPosition.NEGATIVE_MEDIUM:
                        m_PtzManager.CameraZoomWide(m_CamSpeed["ZoomSpeedMedium"]);
                        break;
                    case AxisPosition.NEGATIVE_FAR:
                        m_PtzManager.CameraZoomWide(m_CamSpeed["ZoomSpeedHigh"]);
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

        public void Initialize(IHIDParser hidParser, IPTZManager manager, Dictionary<string, short> camSpeed)
        {
            m_HidParser = hidParser;
            m_PtzManager = manager;
            m_CamSpeed = camSpeed;
            m_ZoomState = new JoystickStateMachine();
            m_PanState = new JoystickStateMachine();
        }


    }
}
