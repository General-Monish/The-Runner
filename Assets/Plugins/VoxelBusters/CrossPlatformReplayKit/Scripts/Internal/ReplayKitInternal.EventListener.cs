using VoxelBusters.ReplayKit.Common.DesignPatterns;

namespace VoxelBusters.ReplayKit
{
    using System;
    using Internal;
    using UnityEngine;

    internal partial class ReplayKitInternal : SingletonPattern<ReplayKitInternal>, INativeCallbackListener
    {
        #region Events

        public  event ReplayKitDelegates.InitialiseCallback                 DidInitialiseEvent;
        public  event ReplayKitDelegates.RecordingStateChangedCallback      DidRecordingStateChangeEvent;
        public event ReplayKitDelegates.RecordingUIActionChangedCallback    DidRecordingUIActionChangeEvent;
        private event ReplayKitDelegates.PreviewStateChangedCallback        DidPreviewStateChangeEvent; // For future updates, we provide preview states. So currently making it private

        #endregion


        #region INativeCallbackListener implementation

        public void OnInitialiseSuccess ()
		{
            Dispatch(() =>
            {
                DidInitialiseEvent?.Invoke(ReplayKitInitialisationState.Success, string.Empty);
            });
		}

        public void OnInitialiseFailed(string message)
        {
            Dispatch(() =>
            {
                DidInitialiseEvent?.Invoke(ReplayKitInitialisationState.Failed, message);
            });
        }

        public void OnRecordingStarted ()
		{

            Dispatch(() =>
            {
                DidRecordingStateChangeEvent?.Invoke(ReplayKitRecordingState.Started, string.Empty);
            });
		}

        public void OnRecordingStopped()
        {
            Dispatch(() =>
            {
                DidRecordingStateChangeEvent?.Invoke(ReplayKitRecordingState.Stopped, string.Empty);
            });
        }

        public void OnRecordingFailed(string message)
        {
            Dispatch(() =>
            {
                DidRecordingStateChangeEvent?.Invoke(ReplayKitRecordingState.Failed, message);
            });
        }

        public void OnRecordingAvailable()
        {
            Dispatch(() =>
            {
                DidRecordingStateChangeEvent?.Invoke(ReplayKitRecordingState.Available, string.Empty);
            });
        }


        public void OnPreviewOpened()
        {
            Dispatch(() =>
            {
                DidPreviewStateChangeEvent?.Invoke(ReplayKitPreviewState.Opened, string.Empty);
            });
        }

        public void OnPreviewClosed()
        {
            Dispatch(() =>
            {
                DidPreviewStateChangeEvent?.Invoke(ReplayKitPreviewState.Closed, string.Empty);
            });
        }

        public void OnPreviewPlayed()
        {
            Dispatch(() =>
            {
                DidPreviewStateChangeEvent?.Invoke(ReplayKitPreviewState.Played, string.Empty);
            });
        }

        public void OnPreviewShared()
        {
            Dispatch(() =>
            {
                DidPreviewStateChangeEvent?.Invoke(ReplayKitPreviewState.Shared, string.Empty);
            });
        }

        public void OnPreviewSaved(string error)
        {
            Dispatch(() =>
            {
                m_savePreviewCallback?.Invoke(string.IsNullOrEmpty(error) ? null : error);
            });
        }

        public void OnRecordingUIStartAction()
        {

            Dispatch(() =>
            {
                DidRecordingUIActionChangeEvent?.Invoke(RecordingUIAction.Started);
            });
        }

        public void OnRecordingUIStopAction()
        {

            Dispatch(() =>
            {
                DidRecordingUIActionChangeEvent?.Invoke(RecordingUIAction.Stopped);
            });
        }

        #endregion

        #region Private methods

        private void Dispatch(Action action)
        {
            UnityThreadDispatcher.Enqueue(action);
        }

        #endregion


    }
}