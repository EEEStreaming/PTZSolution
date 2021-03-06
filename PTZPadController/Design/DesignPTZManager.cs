﻿using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PTZPadController.Design
{
    public class DesignPTZManager : IPTZManager
    {
        public ICameraHandler CameraPreview { get; set; }

        public ICameraHandler CameraProgram { get; set; }

        public List<ICameraHandler> Cameras { get; set; }
        public short CameraSensitivity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddCameraHandler(ICameraHandler camHandler)
        {
        }

        public void AddGamePad(IGamePadHandler pad)
        {
            throw new NotImplementedException();
        }

        public void CameraButtonPresetDown(int preset)
        {
            throw new NotImplementedException();
        }

        public void CameraButtonPresetUp(int preset)
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

        public void CameraZoomTele(short zoomSpeed)
        {
        }

        public void CameraZoomWide(short zoomSpeed)
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

        public void CameraFocusModeAuto()
        {
        }

        public void CameraFocusModeManual()
        {
        }

        public void CameraFocusModeOnePush()
        {
        }

        public void CameraFocusOnePushTrigger()
        {
        }

        public void CameraGetFocusMode()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUp(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUpLeft(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltUpRight(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDown(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDownLeft(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltDownRight(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltLeft(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltRight(short moveSpeed)
        {
            throw new NotImplementedException();
        }

        public void CameraFocusAutoOnePushSwitchMode()
        {
            throw new NotImplementedException();
        }

        public ECameraFocusMode GetCameraFocusMode(string name)
        {
            throw new NotImplementedException();
        }

        public void SaveWindowPosition(Window window, WindowPositionModel winPos)
        {
            throw new NotImplementedException();
        }

        public WindowPositionModel LoadWindowPosition(Window window)
        {
            throw new NotImplementedException();
        }

        public void NextSwitcherPreview()
        {
            throw new NotImplementedException();
        }

        public void CameraPanTiltStop()
        {
            throw new NotImplementedException();
        }
    }
}
