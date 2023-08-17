using UnityEngine;
using System.Collections;
using VoxelBusters.ReplayKit.Common.Utility;
using VoxelBusters.ReplayKit.Common.UASUtils;
using System;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace VoxelBusters.ReplayKit.Internal
{
	public partial class ReplayKitSettings : SharedScriptableObject<ReplayKitSettings>, IAssetStoreProduct
	{
		/// <summary>
		/// Application Settings specific to a platform.
		/// </summary>
		[System.Serializable]
		public class BasePlatformSettings
		{
		}
	}
}