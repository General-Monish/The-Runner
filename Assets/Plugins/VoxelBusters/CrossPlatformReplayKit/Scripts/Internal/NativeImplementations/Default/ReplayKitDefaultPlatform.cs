using UnityEngine;
using System.Collections;

namespace VoxelBusters.ReplayKit.Internal
{
	public partial class ReplayKitDefaultPlatform : MonoBehaviour, INativeService
	{
		private INativeCallbackListener 		m_listener;
        private bool m_isRecording = false;
        private bool m_isPreviewAvailable = false;

        private string m_previewVideoFile = "https://www.youtube.com/watch?v=aqz-KE-bpKQ";

        #region INativeService implementation

        public void Initialise (INativeCallbackListener listener)
		{
			m_listener = listener;

            if (IsRecordingAPIAvailable())
            {
                m_listener.OnInitialiseSuccess();
            }
            else
            {
                m_listener.OnInitialiseFailed("Replay kit API not available");
            }
        }

        public bool IsCameraEnabled()
        {
            return false;
        }

        public bool IsPreviewAvailable()
        {
            return m_isPreviewAvailable;
        }

        public bool IsRecording()
        {
            return m_isRecording;
        }

        public bool IsRecordingAPIAvailable()
        {
            return true;
        }

        public void SetMicrophoneStatus(bool enable)
        {
        }

        public void SetRecordingUIVisibility(bool show)
        {
            Debug.LogWarning("Not implemented on editor. Please check on device for the UI");
        }


        public void PrepareRecording()
        {
            Debug.Log("Preparing for recording...");
        }

        public void StartRecording()
        {
            m_isRecording = true;
            m_listener.OnRecordingStarted();
        }

        public void StopRecording()
        {
            m_isRecording = false;
            m_listener.OnRecordingStopped();
            m_isPreviewAvailable = true;
            m_listener.OnRecordingAvailable();
        }

        public bool Preview()
        {
            if (m_isPreviewAvailable)
            {
                Application.OpenURL(m_previewVideoFile);
                return true;
            }
            else
                return false;
        }

        public bool Discard()
        {
            m_isPreviewAvailable = false;
            return true;
        }

        public string GetPreviewFilePath()
        {
            if(!m_isPreviewAvailable)
            {
                return null;
            }
            else
            {
                return m_previewVideoFile;
            }
        }

        public void SavePreview(string filename = null)
        {
            m_listener.OnPreviewSaved("");
        }

        public void SharePreview(string text = null, string subject = null)
        {
            m_listener.OnPreviewShared();
        }

        #endregion
    }
}