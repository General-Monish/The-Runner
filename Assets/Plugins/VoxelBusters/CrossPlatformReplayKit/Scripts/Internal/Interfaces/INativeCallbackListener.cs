namespace VoxelBusters.ReplayKit.Internal
{
    public interface INativeCallbackListener
    {
        // Initialise Callbacks
        void OnInitialiseSuccess();
        void OnInitialiseFailed(string message);

        // Recording Callbacks
        void OnRecordingStarted();
        void OnRecordingStopped();
        void OnRecordingFailed(string message);
        void OnRecordingAvailable();


        // Preview Callbacks
        void OnPreviewOpened();
        void OnPreviewClosed();
        void OnPreviewShared();
        void OnPreviewSaved(string error);

        // UI Callbacks
        void OnRecordingUIStartAction();
        void OnRecordingUIStopAction();


    }
}
