using PTZPadController.Messages;
using System.Collections.Generic;

namespace PTZPadController.DataAccessLayer
{
    public interface ISwitcherParser
    {
        bool Connected { get; }
        bool IsConnecting { get; }
        
        void Connect();
        void Disconnect();
        void SetPreviewSource(string cameraName);
        void StartTransition(TransitionEnum transition);
        string GetCameraProgramName();
        string GetCameraPreviewName();
        bool FindCameraName(string cameraName);
        void NextSwitcherPreview(List<string> cameraNames);
    }
}