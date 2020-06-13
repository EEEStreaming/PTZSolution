using System;
using System.Threading.Tasks;
using static PTZPadController.BusinessLayer.AtemSwitcherHandler;

namespace PTZPadController.BusinessLayer
{
    public enum Source { 
        Source_1,
        Source_2,
        Source_3,
        Source_4,
    }
    public interface IAtemSwitcherHandler
    {

        void connect();
        /// <summary>
        /// Checks if the switcher is actually connected. Will not wait the completion of a connection if it is in progress.
        /// </summary>
        /// <returns></returns>
        bool isConnected();
        /// <summary>
        /// If a connection is in progress, waits it's completition and then returns the switcher connection status.
        /// </summary>
        /// <returns></returns>
        bool waitForConnection();
        void disconnect();
        public event EventHandler<SourceArgs> PreviewSourceChanged;
        public event EventHandler<SourceArgs> ProgramSourceChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programCamera">Sets the program camera: the actual red camera that is on air.</param>
        void setProgramSource(Source programCamera);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previewCamera">Sets the preview camera: the actual green camera that is about to be on air.</param>
        void setPreviewSource(Source previewCamera);

        void setBothSource(Source programCamera, Source previewCamera) {
            setProgramSource(programCamera);
            setPreviewSource(previewCamera);
        }
    }
}