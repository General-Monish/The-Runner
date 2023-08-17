using UnityEngine;
using System.Collections;
using VoxelBusters.ReplayKit.Common.Utility;
using VoxelBusters.ReplayKit.Common.UASUtils;
using System;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-firstpass")]
#endif

namespace VoxelBusters.ReplayKit.Internal
{
    [AssetCollectionFolderAttribute(Constants.kPluginFolderName)]
    public partial class ReplayKitSettings : SharedScriptableObject<ReplayKitSettings>, IAssetStoreProduct
    {
        #region Constants

        // Product info
        private const string kProductName = "Replay Kit (Free Version with iOS only)";
        private const string kProductVersion = "1.6";

        // Pref key
        internal const string kPrefsKeyPropertyModified = "replaykit-property-modified";
        internal const string kMethodPropertyChanged = "OnPropertyModified";

        #endregion

        #region Fields

        [NonSerialized]
        private AssetStoreProduct m_assetStoreProduct;

        //[SerializeField]
        [Header("Platform Specific Settings")]
        private iOSSettings m_iOS = new iOSSettings();

        //[SerializeField]
        private AndroidSettings m_android = new AndroidSettings();

        [SerializeField]
        [Header("Common Settings")]
        [Header("Upgrade to full version for Android platform support")]
        private bool m_usesMicrophone = true;

        #endregion

        #region Properties

        public AssetStoreProduct AssetStoreProduct
        {
            get
            {
                return m_assetStoreProduct;
            }
        }

        internal iOSSettings IOS
        {
            get
            {
                return m_iOS;
            }
        }

        internal AndroidSettings Android
        {
            get
            {
                return m_android;
            }
        }

        internal bool UsesMicrophone
        {
            get
            {
                return m_usesMicrophone;
            }
        }

        #endregion

        #region Unity Callbacks

        protected override void Reset()
        {
            base.Reset();

#if UNITY_EDITOR
            m_assetStoreProduct = new AssetStoreProduct(kProductName,
                                                        kProductVersion,
                                                        Constants.kLogoPath);
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_EDITOR
            InitialiseEditor();
#endif
        }

        #endregion

        #region Editor Methods

#if UNITY_EDITOR
        private void InitialiseEditor()
        {
            m_assetStoreProduct = new AssetStoreProduct(kProductName,
                                                        kProductVersion,
                                                        Constants.kLogoPath);
        }

        [ContextMenu("Save")]
        public void Rebuild()
        {
            // Actions
            WriteAndroidManifestFile();

            // Refresh Database
            AssetDatabase.Refresh();

            // Reset flags
            EditorPrefs.DeleteKey(kPrefsKeyPropertyModified);
        }

        public void WriteAndroidManifestFile()
        {
            string _manifestFolderPath = Constants.kAndroidPluginsReplayKitPath;

            if (AssetDatabaseUtils.FolderExists(_manifestFolderPath))
            {
                ReplayKitAndroidManifestGenerator _generator = new ReplayKitAndroidManifestGenerator();
#if UNITY_2018_4_OR_NEWER
                _generator.SaveManifest("com.voxelbusters.replaykitplugin", _manifestFolderPath + "/AndroidManifest.xml", "16", "29");
#else
				_generator.SaveManifest("com.voxelbusters.replaykitplugin", _manifestFolderPath + "/AndroidManifest.xml", "16", "26");
#endif
            }
        }
#endif

        #endregion

        #region Editor Callback Methods

#if UNITY_EDITOR

        private void OnPropertyModified()
        {
            EditorPrefs.SetBool(kPrefsKeyPropertyModified, true);
        }

#endif


        #endregion
    }
}