using VoxelBusters.ReplayKit.Common.DesignPatterns;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif


namespace VoxelBusters.ReplayKit
{
    using System;
    using Internal;
    using UnityEngine;

	internal partial class ReplayKitInternal : SingletonPattern<ReplayKitInternal>, INativeCallbackListener
    {
#if UNITY_IOS
	[DllImport("__Internal")]
	protected static extern int UnityShouldAutorotate();
#endif

		INativeService m_service;
        private bool m_enableMicrophone;
        private ReplayKitDelegates.SavePreviewCompleteCallback m_savePreviewCallback;

        private bool m_audioListenerStatus;
        private bool m_isInitialised;

#region Query Methods

        public void Initialise()
        {
            m_isInitialised = true;
            m_service.Initialise(this);
        }

        public bool IsInitialised()
        {
            return m_isInitialised;
        }

        public bool IsRecordingAPIAvailable()
        {
			return m_service.IsRecordingAPIAvailable();
        }

        public bool IsCameraEnabled()
        {
			return m_service.IsCameraEnabled();
        }

        public bool IsRecording()
        {
            return m_service.IsRecording();
        }

        public bool IsMicrophoneEnabled()
        {
            return m_enableMicrophone;
        }

        public bool IsPreviewAvailable()
        {
            return m_service.IsPreviewAvailable();
        }

#endregion

#region Recording Operations

        public void SetMicrophoneStatus(bool enable)
        {
            m_enableMicrophone = enable;
            m_service.SetMicrophoneStatus(enable);
        }

        public void SetRecordingUIVisibility(bool visible)
        {
            m_service.SetRecordingUIVisibility(visible);
        }

        public void PrepareRecording()
        {
            m_service.PrepareRecording();
        }

        public void StartRecording()
        {
            m_service.StartRecording();
        }

        public void StopRecording()
        {
            m_service.StopRecording();
        }

        public bool Preview()
        {
            return m_service.Preview();
        }

        public string GetPreviewFilePath()
        {
            return m_service.GetPreviewFilePath();
        }

        public bool Discard()
        {
            return m_service.Discard();
        }

#endregion

#region Utility

        public void SavePreview(ReplayKitDelegates.SavePreviewCompleteCallback callback)
        {
            m_savePreviewCallback = callback;
            m_service.SavePreview();
        }

        public void SharePreview(string text = null, string subject = null)
        {
            m_service.SharePreview(text, subject);
        }

#endregion


#region Overriden Methods

        protected override void Init()
        {
            base.Init();

			// Not interested in non singleton instance
			if (instance != this)
                return;

#if (UNITY_IOS && !UNITY_EDITOR)
            /*if(UnityShouldAutorotate() == 1)
            {
                m_service = this.gameObject.AddComponent<ReplayKitIOSNormalRecorder>();
            }
            else*/
            {
                m_service = this.gameObject.AddComponent<ReplayKitIOSCustomRecorder>();
            }
#else
            m_service = this.gameObject.AddComponent<ReplayKitDefaultPlatform>();
#endif
        }

#endregion


    }
}