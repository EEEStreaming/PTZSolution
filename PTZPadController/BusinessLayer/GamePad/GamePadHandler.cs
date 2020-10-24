using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.BusinessLayer
{
    public class GamePadHandler : IGamePadHandler
    {
        // This holds the minimum difference value between two commands needed to trigger a state update
        private readonly double CHANGE_THRESHOLD = 0.01;

        private IHIDParser m_HidParser;
        private IPTZManager m_PtzManager;

        // State machines
        private ZoomStateMachine m_ZoomState;
        private PanStateMachine m_PanState;
        private AtemStateMachine m_AtemState;
        private PresetStateMachine m_PresetState;
        private FocusStateMachine m_FocusState;

        // Zoom state
        private double m_currentZoomIndex;
        private readonly double ZoomWFast = 0.1;
        private readonly double ZoomWMedium = 0.2;
        private readonly double ZoomWSlow = 0.4;
        private readonly double ZoomStop = 0.5;
        private readonly double ZoomTSlow = 0.6;
        private readonly double ZoomTMedium = 0.8;
        private readonly double ZoomTFast = 0.9;

        public void Camera1SetPreview(ButtonCommand button)
        {
            if (m_PtzManager.Cameras.Count >= 1)
            {
                if (App.Win != null)
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[0].CameraName));
            }
        }

        public void Camera2SetPreview(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void Camera3SetPreview(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void Camera4SetPreview(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraFocusAutoMode(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraFocusAutoOnePushSwitchMode(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraFocusOnePushMode(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraFocusOnePushTriger(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltAxes(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset1(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset2(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset3(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset4(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset5(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset6(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset7(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void CameraPreset8(ButtonCommand button)
        {
            throw new NotImplementedException();
        }


        public void CameraZoomAxe(double x)
        {
            // Ignore small updates
            if (Math.Max(x, m_currentZoomIndex) - Math.Min(x, m_currentZoomIndex) > CHANGE_THRESHOLD) {
                // Convert value to zoom command
                var transition = x switch
                {
                    double value when value <= ZoomWFast => ZoomCommand.WideFast,
                    double value when value <= ZoomWMedium => ZoomCommand.WideMedium,
                    double value when value <= ZoomWSlow => ZoomCommand.WideSlow,
                    double value when value <= ZoomStop => ZoomCommand.Stop,
                    double value when value <= ZoomTSlow => ZoomCommand.TeleSlow,
                    double value when value <= ZoomTMedium => ZoomCommand.TeleMedium,
                    double value when value <= ZoomTFast => ZoomCommand.TeleFast,
                    _ => ZoomCommand.Stop,
                };
                // Trigger command
                if (m_ZoomState.CurrentState != m_ZoomState.MoveNext(transition))
                {
                    switch (m_ZoomState.CurrentState)
                    {
                        case ZoomState.Inactive:
                            m_PtzManager.CameraZoomStop();
                            break;
                        case ZoomState.ZoomTeleSlow:
                        case ZoomState.ZoomTeleMedium:
                        case ZoomState.ZoomTeleFast:
                            m_PtzManager.CameraZoomTele();
                            break;
                        case ZoomState.ZoomWideFast:
                        case ZoomState.ZoomWideMedium:
                        case ZoomState.ZoomWideSlow:
                            m_PtzManager.CameraZoomWide();
                            break;
                    }
                }
                m_currentZoomIndex = x;
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
            m_ZoomState = new ZoomStateMachine();
            m_currentZoomIndex = 0.5;
        }

        public void SwitcherCut(ButtonCommand button)
        {
            throw new NotImplementedException();
        }

        public void SwitcherMix(ButtonCommand button)
        {
            throw new NotImplementedException();
        }
    }
}
