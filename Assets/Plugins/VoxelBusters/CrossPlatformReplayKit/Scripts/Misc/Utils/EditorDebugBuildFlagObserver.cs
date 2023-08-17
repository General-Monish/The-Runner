using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

namespace VoxelBusters.ReplayKit.Internal
{
	[InitializeOnLoad]
	public class EditorDebugBuildFlagObserver
	{

		#region Fields

		private static bool PreviousDebugFlagStatus = false;

		#endregion

		#region Delegates

		public delegate void OnDebugBuildFlagChanged();

		#endregion

		#region Events

		public static event OnDebugBuildFlagChanged debugBuildFlagChanged;

		#endregion



		static EditorDebugBuildFlagObserver()
		{
			EditorApplication.update -= MonitorDebugFlag;
			EditorApplication.update += MonitorDebugFlag;
		}

		private static void MonitorDebugFlag()
		{
			if(PreviousDebugFlagStatus != EditorUserBuildSettings.development)
			{
				PreviousDebugFlagStatus = EditorUserBuildSettings.development;
				NotifyChangedEvent();
			}
		}

		private static void NotifyChangedEvent()
		{
			if(debugBuildFlagChanged != null)
			{
				debugBuildFlagChanged();
			}
		}
	}
}
#endif
