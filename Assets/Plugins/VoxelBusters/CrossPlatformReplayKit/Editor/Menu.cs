using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using VoxelBusters.ReplayKit.Common.Utility;

namespace VoxelBusters.ReplayKit.Internal
{
	public class Menu 
	{
		#region Constants
	
		// Menu item names
        private 	const 	string 	kMenuNodeMainNode					= "Window/Voxel Busters/Cross Platform Replay Kit";
	
		#endregion

		#region Settings

		[MenuItem(kMenuNodeMainNode + "/Settings", false)]
		private static void SelectSettings ()
		{
			ReplayKitSettings _settings	= ReplayKitSettings.Instance;
			
			if (_settings != null)
			{
				Selection.activeObject	= _settings;
			}
		}

		#endregion

		
#if UNITY_EDITOR && !(UNITY_WEBPLAYER || UNITY_WEBGL || NETFX_CORE)
		[MenuItem(kMenuNodeMainNode + "/Uninstall", false)]
		private static void Uninstall ()
		{				
			UninstallPlugin.Uninstall();
		}
#endif

	}
}
#endif