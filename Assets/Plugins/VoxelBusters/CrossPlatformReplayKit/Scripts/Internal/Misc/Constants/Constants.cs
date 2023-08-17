using UnityEngine;
using System.Collections;

namespace VoxelBusters.ReplayKit.Internal
{
	public class Constants : MonoBehaviour
	{
		#region Errors

		public const string kDebugTag							= "Replay Kit";
		public const string kNotSupportedInEditor				= "The operation could not be completed because the requested feature is not simulated in Unity Editor. Use your mobile device for testing this functionality.";

        #endregion

        #region Assets Path	

        public const string kPluginFolderName                   = "CrossPlatformReplayKit";
		public const string	kRootAssetsPath						= "Assets";
		public const string	kVBCodebasePath						= "Assets/Plugins/VoxelBusters";

        public const string kPluginAssetsPath                   = kVBCodebasePath + "/" + kPluginFolderName;
		public const string kEditorAssetsPath					= kPluginAssetsPath + "/EditorResources";
		public const string kLogoPath							= kEditorAssetsPath + "/Logo/ReplayKit.png";
        public const string kPluginResourcesPath                = kVBCodebasePath + "/PluginResources/" + kPluginFolderName;

        #endregion

        #region GUI Style

        public const string kSampleUISkin						= "AssetStoreProductUISkin";//Available in AssetStoreProduct submodule
		public const string kSubTitleStyle  					= "sub-title";
		public const string	kButtonLeftStyle					= "ButtonLeft";
		public const string	kButtonMidStyle						= "ButtonMid";
		public const string	kButtonRightStyle					= "ButtonRight";

		#endregion

		#region Plugin Paths
        public const string kPluginsAndroidLibraryRootPath      = kRootAssetsPath + "/Plugins/Android";
		public const string kAndroidPluginsReplayKitFolderName	= "com.voxelbusters.replaykit.androidlib";
		public const string kAndroidPluginsReplayKitPath		= kPluginsAndroidLibraryRootPath + "/" + kAndroidPluginsReplayKitFolderName;

		#endregion

		#region Asset Store Constants

		public const string	kFullVersionProductURL				= "http://u3d.as/1nN3";
		public const string kFreeVersionProductURL				= "http://u3d.as/2rkC";

		#endregion
	}
}