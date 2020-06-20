﻿using PTZPadController.Messages;

namespace PTZPadController.DataAccessLayer
{
    public interface ISwitcherParser
    {
        bool Connected { get; }

        void Connect();
        void Disconnect();
        void SetPreviewSource(string cameraName);
        void StartTransition(TransitionEnum transition);
        string GetCameraProgramName();
        string GetCameraPreviewName();
    }
}