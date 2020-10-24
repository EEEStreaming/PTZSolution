using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.BusinessLayer
{
    public class GamePadHandler : IGamePadHandler
    {
        private IHIDParser m_HidParser;
        private IPTZManager m_PtzManager;

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

        public void CameraPanTiltAxes(int x, int y)
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


        public void CameraZoomAxe(int x)
        {
            throw new NotImplementedException();
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
