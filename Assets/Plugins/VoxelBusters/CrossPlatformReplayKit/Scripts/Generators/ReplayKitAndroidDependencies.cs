#if UNITY_EDITOR
using System;
using UnityEditor;
using VoxelBusters.ReplayKit.Common.Utility;
using System.Collections.Generic;
using System.Xml;

namespace VoxelBusters.ReplayKit
{
    using Internal;

    /// <summary>
    /// Replay Kit Dependencies for Cross Platform Replay Kit.
    /// </summary>
    [InitializeOnLoad]
    public class ReplayKitAndroidDependencies
    {
		/// <summary>
		/// The name of your plugin.  This is used to create a settings file
		/// which contains the dependencies specific to your plugin.
		/// </summary>
		private static readonly string DependencyName 		        = "ReplayKitDependencies.xml";

        private static readonly string Path 				        = Constants.kPluginAssetsPath + "/Editor/" + DependencyName;
        private static readonly string AndroidXLibsVersionString    = "1.0.0+";


        /// <summary>
        /// Initializes static members of the <see cref="ReplayKitAndroidDependencies"/> class.
        /// </summary>
        static ReplayKitAndroidDependencies()
		{
			EditorUtils.Invoke(()=>{
				CreateDependencyXML();
			}, 0.1f);
		}

		public static void CreateDependencyXML()
		{
			string _path = Path;

			// Settings
			XmlWriterSettings _settings 	= new XmlWriterSettings();
			_settings.Encoding 				= new System.Text.UTF8Encoding(true);
			_settings.ConformanceLevel 		= ConformanceLevel.Document;
			_settings.Indent 				= true;
			_settings.NewLineOnAttributes 	= true;
			_settings.IndentChars 			= "\t";

			// Generate and write dependecies
			using (XmlWriter _xmlWriter = XmlWriter.Create(_path, _settings))
			{
				_xmlWriter.WriteStartDocument();
				{
                    _xmlWriter.WriteComment("DONT MODIFY HERE. AUTO GENERATED DEPENDENCIES FROM ReplayKitAndroidLibraryDependencies.cs.");

					_xmlWriter.WriteStartElement("dependencies");
					{
						_xmlWriter.WriteStartElement ("androidPackages");
						{

                            _xmlWriter.WriteComment("Dependency added for using AndroidX Libraries");
                            AndroidDependency _androidxCoreDependency = new AndroidDependency("androidx.core", "core", AndroidXLibsVersionString);
                            WritePackageDependency(_xmlWriter, _androidxCoreDependency);

                        }
						_xmlWriter.WriteEndElement();
					}
					_xmlWriter.WriteEndElement();
				}
				_xmlWriter.WriteEndDocument();
			}
		}


		private static void WritePackageDependency(XmlWriter _xmlWriter, AndroidDependency _dependency)
		{
			_xmlWriter.WriteStartElement ("androidPackage");
			{
				_xmlWriter.WriteAttributeString ("spec", String.Format("{0}:{1}:{2}", _dependency.Group, _dependency.Artifact, _dependency.Version));

				List<string> packageIDs = _dependency.PackageIDs;

				if (packageIDs != null) 
				{
					_xmlWriter.WriteStartElement ("androidSdkPackageIds");
					{
						foreach(string _each in packageIDs)
						{
							_xmlWriter.WriteStartElement ("androidSdkPackageId");
							{
								_xmlWriter.WriteString (_each);
							}
							_xmlWriter.WriteEndElement ();
						}
					}
					_xmlWriter.WriteEndElement ();
				}

                List<string> repositories = _dependency.Repositories;


                if (repositories != null) 
                {
                    _xmlWriter.WriteStartElement ("repositories");
                    {
                        foreach(string _each in repositories)
                        {
                            _xmlWriter.WriteStartElement ("repository");
                            {
                                _xmlWriter.WriteString (_each);
                            }
                            _xmlWriter.WriteEndElement ();
                        }
                    }
                    _xmlWriter.WriteEndElement ();
                }

			}
			_xmlWriter.WriteEndElement ();
		}
	}

	public class AndroidDependency
	{
		private string 	m_group;
		private string 	m_artifact;
		private string	m_version;

		private	List<string>	m_packageIDs;
        private List<string>    m_respositories;



		public string Group
		{
			get
			{
				return m_group;
			}
		}

		public string Artifact
		{
			get
			{
				return m_artifact;
			}
		}

		public string Version
		{
			get
			{
				return m_version;
			}
		}

		public List<string> PackageIDs
		{
			get
			{
				return m_packageIDs;
			}
		}

        public List<string> Repositories
        {
            get
            {
                return m_respositories;
            }
        }

		public AndroidDependency(string _group, string _artifact, string _version)
		{
			m_group = _group;
			m_artifact = _artifact;
			m_version = _version;
		}

		public void AddPackageID(string _packageID)
		{
			if (m_packageIDs == null)
				m_packageIDs = new List<string>();


			m_packageIDs.Add(_packageID);
		}

        public void AddRepository(string _repository)
        {
            if (m_respositories == null)
                m_respositories = new List<string>();


            m_respositories.Add(_repository);
        }
	}
}
#endif
