using UnityEngine;
using System;
using System.Collections;


namespace VoxelBusters.ReplayKit
{
	using Internal;

    public class ReplayKitManager
    {
        #region Fields

        private static ReplayKitDelegates.OnRecordingPreviewAvailable m_onRecordingPreviewAvailable;

        #endregion

        #region Events

        public static event ReplayKitDelegates.InitialiseCallback                   DidInitialise;
        public static event ReplayKitDelegates.RecordingStateChangedCallback        DidRecordingStateChange;

		#endregion

		#region Public Methods
		/// <summary>
		/// Initialise Replay Kit
		/// </summary>
		public static void Initialise()
        {
            if (!ReplayKitInternal.Instance.IsInitialised())
            {
                ReplayKitInternal.Instance.DidInitialiseEvent               -= OnDidInitialise;
                ReplayKitInternal.Instance.DidRecordingStateChangeEvent     -= OnDidRecordingStateChange;
                ReplayKitInternal.Instance.DidRecordingUIActionChangeEvent  -= OnDidRecordingUIActionChange;

                ReplayKitInternal.Instance.DidInitialiseEvent               += OnDidInitialise;
                ReplayKitInternal.Instance.DidRecordingStateChangeEvent     += OnDidRecordingStateChange;
				ReplayKitInternal.Instance.DidRecordingUIActionChangeEvent  += OnDidRecordingUIActionChange;

                ReplayKitInternal.Instance.Initialise();
            }
            else
            {
                if(DidInitialise != null)
                {
                    DidInitialise(ReplayKitInitialisationState.Success, "Already initialised");
                }
            }
        }

        /// <summary>
        /// Check if Recording API is available on this platform
        /// </summary>
        /// <returns><c>true</c>, if recording API is available, <c>false</c> otherwise.</returns>
        public static bool IsRecordingAPIAvailable()
        {
            return ReplayKitInternal.Instance.IsRecordingAPIAvailable();
        }

        public static bool IsRecording()
        {
            return ReplayKitInternal.Instance.IsRecording();
        }

        public static bool IsPreviewAvailable()
        {
            return ReplayKitInternal.Instance.IsPreviewAvailable();
        }

        public static void SetMicrophoneStatus(bool enable)
        {
            ReplayKitInternal.Instance.SetMicrophoneStatus(enable);
        }

        [Obsolete("StartRecording(bool enableMicrophone) is deprecated. Please use SetMicrophoneStatus(bool enabled) for controlling microphone for recording and call StartRecording() instead.")]
        public static void StartRecording(bool enableMicrophone)
        {
            SetMicrophoneStatus(enableMicrophone);
            ReplayKitInternal.Instance.StartRecording();
        }

        public static void PrepareRecording()
        {
            ReplayKitInternal.Instance.PrepareRecording();
        }

        public static void StartRecording()
        {
            if (!ReplayKitInternal.Instance.IsInitialised())
            {
                Initialise();
            }

            ReplayKitInternal.Instance.StartRecording();
        }


        public static void StopRecording(ReplayKitDelegates.OnRecordingPreviewAvailable callback = null)
        {
            m_onRecordingPreviewAvailable = callback;
            ReplayKitInternal.Instance.StopRecording();
        }

        public static bool Preview()
        {
            return ReplayKitInternal.Instance.Preview();
        }

        public static string GetPreviewFilePath()
        {
            return ReplayKitInternal.Instance.GetPreviewFilePath();
        }

        public static bool Discard()
        {
            return ReplayKitInternal.Instance.Discard();
        }

        public static void SavePreview(ReplayKitDelegates.SavePreviewCompleteCallback callback)
        {
            ReplayKitInternal.Instance.SavePreview(callback);
        }

        public static void SharePreview()
        {
            ReplayKitInternal.Instance.SharePreview(null);
        }

        #endregion

        #region Private Methods

        private static void ShowRecordingUI()
        {
            ReplayKitInternal.Instance.SetRecordingUIVisibility(true);
        }

        private static void HideRecordingUI()
        {
            ReplayKitInternal.Instance.SetRecordingUIVisibility(false);
        }

        private static bool IsCameraEnabled()
        {
            return ReplayKitInternal.Instance.IsCameraEnabled();
        }

        #endregion

        #region Internal Callbacks

        private static void OnDidInitialise(ReplayKitInitialisationState state, string message)
        {
            DidInitialise?.Invoke(state, message);
        }

        private static void OnDidRecordingStateChange(ReplayKitRecordingState state, string message)
        {
            DidRecordingStateChange?.Invoke(state, message);

            if (state == ReplayKitRecordingState.Available)
            {
                if(m_onRecordingPreviewAvailable != null)
                    m_onRecordingPreviewAvailable(GetPreviewFilePath(), message);

                m_onRecordingPreviewAvailable = null;
            }
            else if (state == ReplayKitRecordingState.Failed)
            {
                if (m_onRecordingPreviewAvailable != null)
                    m_onRecordingPreviewAvailable(null, "Failed while stopping video");

                m_onRecordingPreviewAvailable = null;
            }
        }

        private static void OnDidRecordingUIActionChange(RecordingUIAction action)
		{
            switch(action)
			{
				case RecordingUIAction.Started:
					StartRecording();
					break;
				case RecordingUIAction.Stopped:
					StopRecording();
					break;
				default:
					Debug.LogError("Not implemented");
					break;
			}
		}

        #endregion

    }
}