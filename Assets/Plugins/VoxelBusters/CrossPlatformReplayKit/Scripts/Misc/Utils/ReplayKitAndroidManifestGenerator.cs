using UnityEngine;
using System.Collections;
using VoxelBusters.ReplayKit.Common.Utility;
using System;

#if UNITY_EDITOR
using System.Xml;

namespace VoxelBusters.ReplayKit
{
	using Internal;
	public class ReplayKitAndroidManifestGenerator : AndroidManifestGenerator
	{
		#region Application Methods

		protected override void WriteApplicationProperties (XmlWriter _xmlWriter)
		{
			WriteProviderInfo(_xmlWriter);
            WriteServices(_xmlWriter);
		}

		private void WriteProviderInfo (XmlWriter _xmlWriter)
		{
			// Provider
			_xmlWriter.WriteComment("Custom File Provider. Sharing from internal folders");
			_xmlWriter.WriteStartElement("provider");
			{
                WriteAttributeString(_xmlWriter, "android", "name", null, "com.voxelbusters.replaykit.extensions.FileProviderExtended");
				WriteAttributeString(_xmlWriter, "android", "authorities", null, string.Format("{0}.replaykit.fileprovider", PlayerSettings.GetBundleIdentifier()));

				WriteAttributeString(_xmlWriter, "android", "exported", null, "false");
				WriteAttributeString(_xmlWriter, "android", "grantUriPermissions", null, "true");

				_xmlWriter.WriteStartElement("meta-data");
				{
					WriteAttributeString(_xmlWriter, "android", "name", null, "android.support.FILE_PROVIDER_PATHS");
                    WriteAttributeString(_xmlWriter, "android", "resource", null, "@xml/replay_kit_file_paths");
				}
				_xmlWriter.WriteEndElement();
			}
			_xmlWriter.WriteEndElement();
		}

		#endregion

		#region Permission Methods

		protected override void WritePermissions (XmlWriter _xmlWriter)
		{
            if (ReplayKitSettings.Instance.UsesMicrophone)
            {
                //Record Audio Permission
                WriteUsesPermission(_xmlWriter: _xmlWriter,
                                _name: "android.permission.RECORD_AUDIO",
                                _comment: "Required for recording audio");
            }

            if (ReplayKitSettings.Instance.Android.UsesSavePreview)
            {
                WriteUsesPermission(_xmlWriter: _xmlWriter,
                                    _name: "android.permission.READ_EXTERNAL_STORAGE",
									_toolsOption : "android:maxSdkVersion",
                                    _comment: "For reading files from external storage");

                WriteUsesPermission(_xmlWriter: _xmlWriter,
                                    _name: "android.permission.WRITE_EXTERNAL_STORAGE",
									_toolsOption : "android:maxSdkVersion",
                                    _comment: "For storing files in external storage");
            }

            WriteUsesPermission(_xmlWriter: _xmlWriter,
                                _name:      "android.permission.FOREGROUND_SERVICE",
                                _comment:   "Required starting from TargetAPI 29");
			

            //Camera Permission
            /*WriteUsesPermission(_xmlWriter: _xmlWriter,
                                _name: "android.permission.CAMERA",
                                _comment: "Required for internet access");*/
        }

        #endregion

        #region Services

        protected void WriteServices(XmlWriter _xmlWriter)
        {
            WriteService(_xmlWriter,
                       "com.voxelbusters.replaykit.internal.ScreenRecordingService",
                       _enabled: true,
                       _foregroundServiceType: "mediaProjection") ;
        }

        #endregion
    }
}
#endif
