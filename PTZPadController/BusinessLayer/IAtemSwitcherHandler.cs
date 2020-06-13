using System.Threading.Tasks;

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

        Task connect();
        void disconnect();
        void onPreviewSourceChange();
        void onProgramSourceChange();
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