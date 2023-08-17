using UnityEngine;
using System.Collections;

namespace VoxelBusters.ReplayKit
{
    public enum ReplayKitInitialisationState
    {
        /// <summary>
        /// State when initialisation is success
        /// </summary>
        Success = 0,
        /// <summary>
        /// State when initialisation is failed
        /// </summary>
        Failed,
    }


    public enum ReplayKitRecordingState
    {
        /// <summary>
        /// State when recording starts
        /// </summary>
        Started = 0,
        /// <summary>
        /// State when recording ends
        /// </summary>
        Stopped,
        /// <summary>
        /// State when recording fails
        /// </summary>
        Failed,
        /// <summary>
        /// Recording Available for preview after record action
        /// </summary>
        Available
    }

    public enum ReplayKitPreviewState
    {
        /// <summary>
        /// State when preview opens successfully
        /// </summary>
        Opened = 0,
        /// <summary>
        /// State when preview is closed
        /// </summary>
        Closed,
        /// <summary>
        /// State when preview failes to open
        /// </summary>
        Failed,
        /// <summary>
        /// State when share button is clicked in the preview
        /// </summary>
        Shared,
        /// <summary>
        /// State when user started playing preview
        /// </summary>
        Played
    }

    public enum VideoQuality
    {
        /// <summary>
        /// Quality setting for matching the screen resolution
        /// </summary>
        QUALITY_MATCH_SCREEN_SIZE = -1,
        /// <summary>
        /// Quality setting for 1080p resolution
        /// </summary>
        QUALITY_1080P,
        /// <summary>
        /// Quality setting for 720p resolution
        /// </summary>
        QUALITY_720P,
        /// <summary>
        /// Quality setting for 480p resolution
        /// </summary>
        QUALITY_480P
    }


    public enum RecordingUIAction
    {
        Started,
        Stopped
    }
}