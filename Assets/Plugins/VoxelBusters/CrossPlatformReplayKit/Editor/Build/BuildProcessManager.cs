using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace VoxelBusters.ReplayKit.Internal
{
    internal static class BuildProcessManager
    {
#region Static methods

        public static void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            switch (newTarget)
            {
                case BuildTarget.iOS:
                    break;

                case BuildTarget.Android:
                    OnAndroidPlatformSelected();
                    break;

                default:
                    break;
            }
        }

        public static void OnPreprocessBuild(BuildInfo buildInfo)
        {
            // invoke appropriate handlers
            switch (buildInfo.Target)
            {
                case BuildTarget.iOS:
                    PreprocessBuild_iOS(buildInfo);
                    break;

                case BuildTarget.Android:
                    PreprocessBuild_Android(buildInfo);
                    break;
                default:
                    break;
            }
        }

        public static void OnPostprocessBuild(BuildInfo buildInfo)
        {
            switch (buildInfo.Target)
            {
                case BuildTarget.iOS:
                    PostprocessBuild_iOS(buildInfo);
                    break;

                default:
                    break;
            }
        }

#endregion

#region iOS methods

        private static void PreprocessBuild_iOS(BuildInfo buildInfo)
        {
           
        }

        private static void PostprocessBuild_iOS(BuildInfo buildInfo)
        {
            Debug.Log("[ReplayKit] iOS build postprocess task is starting now.");
            UpdateXcodeProject(buildInfo);
            UpdateInfoPlist(buildInfo);
            Debug.Log("[ReplayKit] iOS build postprocess task has been completed.");
        }

        private static void UpdateXcodeProject(BuildInfo buildInfo)
        {
        }

        private static void UpdateInfoPlist(BuildInfo buildInfo)
        {
#if UNITY_IOS
            Debug.Log("[ReplayKit] Updating info.plist.");

            // open plist
            string              plistPath   = buildInfo.Path + "/Info.plist";
            PlistDocument       plist       = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict    rootDict    = plist.root;

            // add usage permissions
            Dictionary<string, string> permissions = GetUsagePermissions();
            foreach (string key in permissions.Keys)
            {
                rootDict.SetString(key, permissions[key]);
            }
        
            // save changes to file
            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }


        private static Dictionary<string, string> GetUsagePermissions()
        {
            Dictionary<string, string>      requiredPermissionsDict     = new Dictionary<string, string>(4);
            requiredPermissionsDict[InfoPlistKeys.kNSPhotoLibraryUsage] = "This app saves videos to your Photo Library.";
            requiredPermissionsDict[InfoPlistKeys.kNSPhotoLibraryAdd]   = "This app saves videos to your Photo Library.";
            requiredPermissionsDict[InfoPlistKeys.kNSMicrophoneUsage]   = "This app uses microphone while recording videos.";

            return requiredPermissionsDict;
        }

        private static string[] GetApplicationQueriesSchemes()
        {
            List<string> schemeList = new List<string>();
            schemeList.Add("fb");
            schemeList.Add("twitter");
            schemeList.Add("whatsapp");
            
            return schemeList.ToArray();
        }

#endregion

#region Android methods

        private static void OnAndroidPlatformSelected()
        {
        }

        private static void PreprocessBuild_Android(BuildInfo buildInfo)
        {
            PerformCommonTasksForAndroidExport();

            // Warn missing details and abort if not met
        }

        private static void PerformCommonTasksForAndroidExport()
        {
            // Regenerate manifest with the latest settings
            ReplayKitSettings.Instance.WriteAndroidManifestFile();

            // Enable required libraries


            // Generate any config files (ex : Firebase)
        }

#endregion

#region Nested types

        internal class InfoPlistKeys
        {
#region Constants

            internal const string   kNSContactsUsage                    = "NSContactsUsageDescription";
            internal const string   kNSCameraUsage                      = "NSCameraUsageDescription";
            internal const string   kNSPhotoLibraryUsage                = "NSPhotoLibraryUsageDescription";
            internal const string   kNSPhotoLibraryAdd                  = "NSPhotoLibraryAddUsageDescription";
            internal const string   kNSLocationWhenInUse                = "NSLocationWhenInUseUsageDescription";

            internal const string   kNSAppTransportSecurity             = "NSAppTransportSecurity";
            internal const string   kNSAllowsArbitraryLoads             = "NSAllowsArbitraryLoads";
            internal const string   kNSQuerySchemes                     = "LSApplicationQueriesSchemes";
            internal const string   kNSDeviceCapablities                = "UIRequiredDeviceCapabilities";
            internal const string   kNSMicrophoneUsage                  = "NSMicrophoneUsageDescription";


#endregion
        }

#endregion
    }
}
#endif