namespace PTZPadController.BusinessLayer
{
    public enum Camera { 
        Camera_1,
        Camera_2,
        Camera_3,
        Camera_4,
    }
    public interface IAtemSwitcherHandler
    {
        
        void connect();
        void disconnect();
        void onPreviewSourceChange();
        void onProgramSourceChange();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programCamera">Sets the program camera: the actual red camera that is on air.</param>
        void setProgramSource(Camera programCamera);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previewCamera">Sets the preview camera: the actual green camera that is about to be on air.</param>
        void setPreviewSource(Camera previewCamera);

        void setBothSource(Camera programCamera, Camera previewCamera) {
            setProgramSource(programCamera);
            setPreviewSource(previewCamera);
        }
    }
}