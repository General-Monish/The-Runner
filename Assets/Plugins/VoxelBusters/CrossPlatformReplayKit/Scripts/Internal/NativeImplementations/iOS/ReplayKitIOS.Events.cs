using UnityEngine;
using System.Collections;
using VoxelBusters.ReplayKit.Common.Utility;

#if UNITY_IOS
namespace VoxelBusters.ReplayKit.Internal
{
	public partial class ReplayKitIOS : MonoBehaviour, INativeService
	{
        public void OnReplayKitRecordingStopped(string message)
        {
            m_listener.OnRecordingStopped();
        }

        public void OnReplayKitRecordingStarted(string message)
        {
            m_listener.OnRecordingStarted();
        }

        public void OnReplayKitSaveToGalleryFinished(string message)
        {
            m_listener.OnPreviewSaved(message);
        }

        public void OnReplayKitRecordingAvailable(string message)
        {
            m_listener.OnRecordingAvailable();
        }

        public void OnReplayKitRecordingFailed(string message)
        {
            m_listener.OnRecordingFailed(message);
        }

        public void OnReplayKitRecordingUIStartAction(string message)
        {
            m_listener.OnRecordingUIStartAction();
        }

        public void OnReplayKitRecordingUIStopAction(string message)
        {
            m_listener.OnRecordingUIStopAction();
        }

    }
}
#endif