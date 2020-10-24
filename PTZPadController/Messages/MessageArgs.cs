using Microsoft.Xaml.Behaviors.Media;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.Messages
{
    public class NotificationSource
    {
        public const string ProgramSourceChanged = "ProgramSourceChanged";  // with AtemSourceMessageArgs
        public const string PreviewSourceChanged = "PreviewSourceChanged";  // with AtemSourceMessageArgs
        public const string CameraStatusChanged = "CameraStatusChanged";  // with CameraStatusMessageArgs
        public const string CameraFocusModeChanged = "CameraFocusModeChanged";  // with CameraFocusModeMessageArgs

        public const string SwictcherConnected = "SwictcherConnected"; //
        public const string SocketConnected = "SocketConnected"; //
    }

    public enum CameraStatusEnum
    {
        Preview,
        Program,
        Off
    }

    public enum TransitionEnum
    {
        Cut,
        Mix
    }

    /// <summary>
    /// Message used with notification CameraStatusChanged
    /// </summary>    
    public class CameraStatusMessageArgs
    {
        public CameraStatusEnum Status { get; set; }
        public string CameraName { get; set; }
    }

    /// <summary>
    /// Message used with notification CameraStatusChanged
    /// </summary>    
    public class CameraFocusModeMessageArgs
    {
        public EFocusMode Focus { get; set; }
        public string CameraName { get; set; }
    }


    /// <summary>
    /// Message used with notification ProgramSourceChanged and PreviewSourceChanged
    /// </summary>
    public class AtemSourceMessageArgs 
    {
        public string PreviousInputName { get; set; }
        public string CurrentInputName { get; set; }
    }

   
}
