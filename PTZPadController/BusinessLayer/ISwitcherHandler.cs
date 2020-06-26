using PTZPadController.Messages;
using System;
using System.Threading.Tasks;
using static PTZPadController.BusinessLayer.AtemSwitcherHandler;

namespace PTZPadController.BusinessLayer
{
    public interface ISwitcherHandler
    {

        void ConnectTo();
        /// <summary>
        /// Checks if the switcher is actually connected. Will not wait the completion of a connection if it is in progress.
        /// </summary>
        /// <returns></returns>
        //public bool IsConnected { get; }

        /// <summary>
        /// If a connection is in progress, waits it's completition and then returns the switcher connection status.
        /// </summary>
        /// <returns></returns>
        bool WaitForConnection();
        void Disconnect();

        /// <summary>
        /// Sets the preview camera
        /// </summary>
        /// <param name="cameraName">the actual green camera that is about to be on air.</param>
        void SetPreviewSource(string cameraName);

        void StartTransition(TransitionEnum transition);

        public string GetCameraProgramName();
        public string GetCameraPreviewName();
        bool FindCameraName(string cameraName);
    }
}