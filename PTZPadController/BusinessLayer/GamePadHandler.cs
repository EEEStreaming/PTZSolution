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

        public void CameraCallPreset(int preset)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDown()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDownLeft()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDownRight()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltLeft()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltRight()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltStop()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUp()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUpLeft()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUpRight()
        {
            throw new NotImplementedException();
        }

        public void CameraSetPreset(int preset)
        {
            throw new NotImplementedException();
        }

        public void CameraSetPreview(int cameraid)
        {
            if (m_PtzManager.Cameras.Count >= cameraid)
            {
                if (App.Win != null) 
                    App.Win.Dispatcher.Invoke(() => m_PtzManager.SetSwitcherPreview(m_PtzManager.Cameras[cameraid - 1].CameraName));
            }
        }

        public void CameraZoomStop()
        {
            throw new NotImplementedException();
        }

        public void CameraZoomTele()
        {
            throw new NotImplementedException();
        }

        public void CameraZoomWide()
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
    }
}
