﻿using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

namespace VoxelBusters.ReplayKit.Common.Utility
{
	public class AssetDatabaseUtils 
	{
		#region Constants

		private 	const 		string			kAssets		= "Assets";

		#endregion

		#region Asset Path Methods

		/// <summary>
		/// Gets the project path.
		/// </summary>
		/// <returns>The project path.</returns>
		public static string GetProjectPath ()
		{
			return Path.GetFullPath(Application.dataPath + @"/../");
		}

		/// <summary>
		/// Translate the asset's relative path to absolute path.
		/// </summary>
		/// <returns>The absolute path to asset.</returns>
		/// <param name="_relativePath">Path name relative to the project folder where the asset is stored, for example: "Assets/example.png".</param>
		public static string AssetPathToAbsolutePath (string _relativePath)
		{
			string _unrootedRelativePath	= _relativePath.TrimStart('/');

			if (!_unrootedRelativePath.StartsWith(kAssets))
				return null;

			string _absolutePath			= Path.Combine(GetProjectPath(), _unrootedRelativePath);

			// Return absolute path to asset
			return _absolutePath;
		}

		/// <summary>
		/// Translate a GUID to its absolute path.
		/// </summary>
		/// <returns>The absolute path to asset.</returns>
		/// <param name="_guid">GUID for the asset.</param>
		public static string GUIDToAssetAbsolutePath (string _guid)
		{
			string 		_relativePath		= AssetDatabase.GUIDToAssetPath(_guid);

			if (string.IsNullOrEmpty(_relativePath))
				return null;
			
			return AssetPathToAbsolutePath(_relativePath);
		}

		public static bool FolderExists (string _assetFolderPath)
		{
			return Directory.Exists(AssetPathToAbsolutePath(_assetFolderPath));
		}

		public static string CreateFolder(string _path)
		{
			string[]	_pathComponents		= _path.Split('/');
			string		_parentFolder		= _pathComponents[0];

			int		_pIter		= 1;
			string	_assetGUID	= null;
			while (_pIter < _pathComponents.Length)
			{
				string	_currentPath	= _parentFolder + "/" + _pathComponents[_pIter];
				if (AssetDatabase.IsValidFolder(_currentPath))
				{
					_assetGUID	= AssetDatabase.AssetPathToGUID(_currentPath);
				}
				else
				{
					_assetGUID	= AssetDatabase.CreateFolder(_parentFolder, _pathComponents[_pIter]);
				}

				_parentFolder	= _currentPath;
				_pIter++;
			}

			return _assetGUID;
		}

		#endregion
	}
}
#endif