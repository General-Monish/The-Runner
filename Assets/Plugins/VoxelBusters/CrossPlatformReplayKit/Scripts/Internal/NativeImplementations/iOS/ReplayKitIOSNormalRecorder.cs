
#if UNITY_IOS
using UnityEngine;
using System.Collections;

namespace VoxelBusters.ReplayKit.Internal
{
    public class ReplayKitIOSNormalRecorder : ReplayKitIOS
    {
        private string m_cachedFilePath = null;
        public override bool IsRecordingAPIAvailable()
        {
            return UnityEngine.Apple.ReplayKit.ReplayKit.APIAvailable;
        }

        public override bool IsRecording()
        {
            return UnityEngine.Apple.ReplayKit.ReplayKit.isRecording;
        }

        public override bool IsPreviewAvailable()
        {
            return !string.IsNullOrEmpty(m_cachedFilePath);   
        }

        public override void PrepareRecording()
        {
            replaykit_prepareRecording();
        }

        public override void StartRecording()
        {
            if (IsPreviewAvailable())
                Discard();

            bool status = UnityEngine.Apple.ReplayKit.ReplayKit.StartRecording(m_isMicrophoneEnabeld);
            if(status)
            {
                m_listener.OnRecordingStarted();
            }
            else
            {
                Debug.LogError("Recording failed : " + UnityEngine.Apple.ReplayKit.ReplayKit.lastError);
                m_listener.OnRecordingFailed(IsRecordingAPIAvailable() ? "START_RECORDING_FAILED"  : "API_UNAVAILABLE");
            }
        }

        public override void StopRecording()
        {
            bool status = UnityEngine.Apple.ReplayKit.ReplayKit.StopRecording();

            StartCoroutine(CheckVideoAvailable(status));
        }

        public override bool Preview()
        {
            return replaykit_previewRecording();
                //UnityEngine.Apple.ReplayKit.ReplayKit.Preview();
        }

        public override bool Discard()
        {
            m_cachedFilePath = null;
            return UnityEngine.Apple.ReplayKit.ReplayKit.Discard();
        }

        public override string GetPreviewFilePath()
        {
            return m_cachedFilePath;
        }

        public override void SavePreview(string filename = null)
        {
            string previewFilePath = m_cachedFilePath;

            if (!string.IsNullOrEmpty(previewFilePath))
            {
                replaykit_savePreview(previewFilePath);
            }
            else
            {
                Debug.LogError("No preview recording available for saving!");
                return;
            }

        }

        public override void SharePreview(string text = null, string subject = null)
        {
            replaykit_sharePreview(text, subject);
        }

        public override void SetMicrophoneStatus(bool enable)
        {
            base.SetMicrophoneStatus(enable);
        }

        public override void SetRecordingUIVisibility(bool visible)
        {
            replaykit_setRecordingUIVisibility(visible);
        }

        private IEnumerator CheckVideoAvailable(bool status)
        {
            yield return new WaitForSeconds(0.3f);

            while (status && string.IsNullOrEmpty(UnityEngine.Apple.ReplayKit.ReplayKit.lastError) && !UnityEngine.Apple.ReplayKit.ReplayKit.recordingAvailable)
            {
                yield return new WaitForSeconds(0.3f);
            }

            if (status && string.IsNullOrEmpty(UnityEngine.Apple.ReplayKit.ReplayKit.lastError))
            {
                m_listener.OnRecordingStopped();
                m_cachedFilePath = replaykit_getPreviewFilePath();
                m_listener.OnRecordingAvailable();//This is required as in future we may have some video processing (Audio+Video Mux)
            }
            else
            {
                Discard();
                Debug.LogError("Recording failed : " + UnityEngine.Apple.ReplayKit.ReplayKit.lastError);
                m_listener.OnRecordingFailed(IsRecordingAPIAvailable() ? "STOP_RECORDING_FAILED" : "API_UNAVAILABLE");
            }
        }
    }
}

#endif