using UnityEngine;
using System.Collections;

namespace VoxelBusters.ReplayKit
{
	using Internal;
    /// <summary>
    /// ReplayKitDelegates contains all the delegates required for ReplayKit callbacks.
    /// </summary>
    public class ReplayKitDelegates
	{
        #region Delegates

        /// <summary>
        /// Delegate that will be called upon completion of Initialise() method.
        /// </summary>
        /// <param name="state">Contains the eReplayKitInitialisationState state value.</param>
        /// <param name="message">String that describes the reason for the state, if any.</param>
        public delegate void InitialiseCallback(ReplayKitInitialisationState state, string message);

        /// <summary>
        /// Delegate that will be called when there is a change in recording state.
        /// </summary>
        /// <param name="state">Contains the eReplayKitRecordingState state value.</param>
        /// <param name="message">String that describes the reason for the state, if any.</param>
        public delegate void RecordingStateChangedCallback(ReplayKitRecordingState state, string message);

        /// <summary>
        /// Delegate that will be called when there is a change while previewing.
        /// </summary>
        /// <param name="state">Contains the eReplayKitPreviewState state value.</param>
        /// <param name="message">String that describes the reason for the state, if any.</param>
        public delegate void PreviewStateChangedCallback(ReplayKitPreviewState state, string message);


        /// <summary>
        /// Delegate that will be called when the save of preview recording is complete.
        /// </summary>
        /// <param name="error">Error message code if fails to save</param>
        public delegate void SavePreviewCompleteCallback(string error);


        public delegate void RecordingUIActionChangedCallback(RecordingUIAction action);

        public delegate void OnRecordingPreviewAvailable(string path, string error);

        #endregion
    }
}