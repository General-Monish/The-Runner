using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelBusters.ReplayKit.Common.Utility
{
    public abstract class SharedScriptableObject<T> : ScriptableObject, ISaveAssetCallback where T : ScriptableObject
    {
        #region Static Fields

        private static T instance = null;

        #endregion

        #region Constant Fields

        private const string kDefaultFolderName = "SharedAssets";
        private const string kDefaultPathFormat = "Assets/Resources/{0}/{1}.asset";

        #endregion

        #region Static Properties

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string _path = string.Format(kDefaultPathFormat, GetAssetFolderName(typeof(T)), typeof(T).Name);

#if UNITY_EDITOR
                    // check whether destination folder exists
                    if (!AssetDatabase.IsValidFolder(_path))
                    {
                        AssetDatabaseUtils.CreateFolder(_path.Replace(string.Format("/{0}.asset", typeof(T).Name), ""));
                    }
#endif
                    instance = ScriptableObjectUtils.LoadAssetAtPath<T>(_path);

#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = ScriptableObjectUtils.Create<T>(_path);
                    }
#endif
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        #endregion

        #region Public Methods

#if UNITY_EDITOR
        public void Save()
        {
            OnBeforeSave();

            this.SaveChanges();
        }
#endif

        #endregion

        #region Unity Callbacks

        protected virtual void Reset()
        { }

        protected virtual void OnEnable()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }

        protected virtual void OnDisable()
        { }

        protected virtual void OnDestroy()
        { }

        #endregion

        #region ISaveAssetCallback Implementation

        public virtual void OnBeforeSave()
        { }

        #endregion

        #region Helpers

        internal static string GetAssetFolderName(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(AssetCollectionFolderAttribute), inherit: false);
            if (attributes.Length > 0)
            {
                string folderName = ((AssetCollectionFolderAttribute)attributes[0]).FolderName;
                return folderName;
            }

            return kDefaultFolderName;
        } 
        #endregion
    }
}