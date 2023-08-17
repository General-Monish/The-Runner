#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using PlayerSettings	= VoxelBusters.ReplayKit.Common.Utility.PlayerSettings;

namespace VoxelBusters.ReplayKit.Internal
{
	[InitializeOnLoad]
	public class ReplayKitEditorManager : MonoBehaviour 
	{
		#region Constants

		private		const	string		kPrefsKeyBuildIdentifier	= "replay-kit-build-identifier";

		private		const	float 		kWaitingPeriod 	= 2f;

		#endregion

		#region Static Fields

		private		static 		float 		startTime;

		#endregion

		#region Static Constructor

		#if !REPLAY_KIT_HIBERNATE
        static ReplayKitEditorManager()
		{
			Reset();

			// regiser to editor update callback
			EditorApplication.update   += EditorUpdate;
		}
		#endif

		#endregion

		#region Static Methods

		private static void Reset()
		{
			EditorApplication.update   -= EditorUpdate;

			// set default properties
			startTime 					= (float)EditorApplication.timeSinceStartup;

			// Update manifest to use development client id when debug is used
			EditorDebugBuildFlagObserver.debugBuildFlagChanged -= RebuildSettings;
			EditorDebugBuildFlagObserver.debugBuildFlagChanged += RebuildSettings;
		}

		private static void EditorUpdate()
		{
			if (GetTimeSinceStart() < kWaitingPeriod)
				return;

			MonitorPlayerSettings();
		}

		private static float GetTimeSinceStart()
		{
			return (float)(EditorApplication.timeSinceStartup - startTime);
		}

		private static void MonitorPlayerSettings()
		{
			// check whether there's change in value
			string	_oldBuildIdentifier	= EditorPrefs.GetString(kPrefsKeyBuildIdentifier, null);
			string	_curBuildIdentifier	= PlayerSettings.GetBundleIdentifier();
			if (string.Equals(_oldBuildIdentifier, _curBuildIdentifier))
				return;

			// save copy of new value
			EditorPrefs.SetString(kPrefsKeyBuildIdentifier, _curBuildIdentifier);

			RebuildSettings();
		}

		private static void RebuildSettings()
		{
			// rebuild associated files
			ReplayKitSettings _settings	= ReplayKitSettings.Instance;
			if (_settings != null)
				_settings.Rebuild();
		}

		#endregion
	}
}
#endif