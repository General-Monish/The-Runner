using UnityEngine;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;
using System.IO;

namespace VoxelBusters.ReplayKit.Internal
{
    public abstract partial class ReplayKitIOS : MonoBehaviour, INativeService
    {
        protected INativeCallbackListener m_listener = null;
        protected bool m_isMicrophoneEnabeld = false;

#region Native Methods

        [DllImport("__Internal")]
        protected static extern void replaykit_prepareRecording();

        [DllImport("__Internal")]
        protected static extern void replaykit_startRecording();

        [DllImport("__Internal")]
        protected static extern void replaykit_stopRecording();

        [DllImport("__Internal")]
        protected static extern string replaykit_getPreviewFilePath();

        [DllImport("__Internal")]
        protected static extern bool replaykit_isAPIAvailable();

        [DllImport("__Internal")]
        protected static extern bool replaykit_isRecording();

        [DllImport("__Internal")]
        protected static extern bool replaykit_isPreviewAvailable();

        [DllImport("__Internal")]
        protected static extern bool replaykit_previewRecording();

        [DllImport("__Internal")]
        protected static extern void replaykit_sharePreview (string text, string subject);

        [DllImport("__Internal")]
        protected static extern void replaykit_savePreview (string filename);

        [DllImport("__Internal")]
        protected static extern bool replaykit_discardRecording ();

        [DllImport("__Internal")]
        protected static extern void replaykit_setRecordingUIVisibility(bool visible);

        [DllImport("__Internal")]
        protected static extern void replaykit_setMicrophoneStatus(bool isEnabled);

#endregion

#region INativeService implementation

        public void Initialise(INativeCallbackListener listener)
        {
            m_listener = listener;
            IsRecordingAPIAvailable();
            m_listener.OnInitialiseSuccess();
        }

        public abstract bool IsRecordingAPIAvailable();
        public abstract bool IsRecording();
        public abstract bool IsPreviewAvailable();
        public abstract void PrepareRecording();
        public abstract void StartRecording();
        public abstract void StopRecording();
        public abstract bool Preview();
        public abstract bool Discard();
        public abstract string GetPreviewFilePath();
        public abstract void SavePreview(string filename = null);
        public abstract void SharePreview(string text = null, string subject = null);
        public abstract void SetRecordingUIVisibility(bool visible);

        public virtual void SetMicrophoneStatus(bool enable)
        {
            m_isMicrophoneEnabeld = enable;
        }

        public bool IsCameraEnabled()
        {
            return false;
        }

#endregion
    }
}
#endif