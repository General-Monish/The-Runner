#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using VoxelBusters.ReplayKit.Common.Utility;
using VoxelBusters.ReplayKit.Common.UASUtils;

using Array		= System.Array;

namespace VoxelBusters.ReplayKit.Internal
{
	[CustomEditor(typeof(ReplayKitSettings))]
	public class ReplayKitSettingsEditor : AssetStoreProductInspector
	{
		#region Constants

		private		const 	string			kActiveView						= "replaykit-active-view";

		// URL
		private 	const	string			kSupportURL						= "https://discord.gg/jegTXvqPKQ";
		private		const 	string			kTutorialURL					= "https://assetstore.replaykit.voxelbusters.com";		
		private		const	string			kSubscribePageURL			    = "http://bit.ly/2ESQfAg";

		// style
		private		const	int				kTitleFontSize					= 16;
		private		const	int				kSubTitleFontSize				= 12;
		private		const	int				kGridRowCount					= 2;

		#endregion

		#region Properties

		// GUI contents
#pragma warning disable
		private		GUIContent				m_upgradeContent			= new GUIContent("Upgrade", "Upgrade to full version");
		private 	GUIContent				m_supportContent			= new GUIContent("Support",	"Houston, we have a problem!");
		private 	GUIContent				m_tutotialsContent			= new GUIContent("Tutorials", 		"Read our blog posts about product features and usage");
		private		GUIContent				m_writeReviewContent		= new GUIContent("Review", 			"Share your experience with others");
		private 	GUIContent				m_subscribeContent			= new GUIContent("Subscribe", 		"Stay updated regarding bug fixes and releases");
		private 	GUIContent				m_saveChangesContent		= new GUIContent("Save", 			"Save all your changes");
#pragma warning restore

		#endregion

		#region Unity Callbacks

		private void OnInspectorUpdate() 
		{
			// Call Repaint on OnInspectorUpdate as it repaints the windows
			// less times as if it was OnGUI/Update
			Repaint();

			//DrawDefaultInspector();

		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnGUIWindow()
		{
			// draw inspector properties
			GUILayout.BeginVertical(UnityEditorUtility.kOuterContainerStyle);
			{	
				base.OnGUIWindow();

				// disable inspector when its compiling
				GUI.enabled	= !EditorApplication.isCompiling;

				UnityEditorUtility.DrawSplitter(new Color(0.35f, 0.35f, 0.35f), 1, 10);
				DrawTopBar();
				GUILayout.Space(10f);

				/*if (m_activeType == eSettingsType.NONE)
				{
					DrawMainMenu(_options: GetAvailableTabs());
				}
				else
				{
					DrawDetailedView();
				}*/
				DrawDetailedView();

				GUILayout.Space(10f);
				DrawFooter();
				GUILayout.Space(10f);
			}
			GUILayout.EndVertical();


		//	DrawDefaultInspector();

			// reset GUI state
			GUI.enabled	= true;
		}

		#endregion

		#region Misc. Methods

		private void DrawTopBar()
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(m_upgradeContent, Constants.kButtonLeftStyle))
					Application.OpenURL(Constants.kFullVersionProductURL);

				if (GUILayout.Button(m_tutotialsContent, Constants.kButtonMidStyle))
					Application.OpenURL(kTutorialURL);

				if (GUILayout.Button(m_supportContent, Constants.kButtonMidStyle))
					Application.OpenURL(kSupportURL);

				if (GUILayout.Button(m_writeReviewContent, Constants.kButtonMidStyle))
					Application.OpenURL(Constants.kFreeVersionProductURL);

				if (GUILayout.Button(m_subscribeContent, Constants.kButtonMidStyle))
					Application.OpenURL(kSubscribePageURL);

				if (GUILayout.Button(m_saveChangesContent, Constants.kButtonRightStyle))
					OnPressingSave();
			}
			GUILayout.EndHorizontal();
		}


		private void DrawDetailedView()
		{

			// title section
			GUILayout.BeginHorizontal();
			GUILayout.Space(4f);

			//			GUILayout.EndVertical();
			//			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);


			SerializedProperty prop = serializedObject.GetIterator();
			if (prop.NextVisible(true)) {
				do 
				{
					if (prop.hasChildren && prop.propertyType != SerializedPropertyType.String)
					{
						GUILayout.Label(prop.displayName, "OL Minus");
						UnityEditorUtility.ForEach(prop, (_innerChildProperty) =>
							{
								EditorGUI.indentLevel++;
								EditorGUILayout.PropertyField(_innerChildProperty, true);
								EditorGUI.indentLevel--;
							});
					}
					else
					{
						EditorGUILayout.PropertyField(prop);
					}
				}
				while (prop.NextVisible(false));
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawFooter()
		{
			Color	_oldColor 		= GUI.color;

			bool	_isModified 	= EditorPrefs.GetBool(ReplayKitSettings.kPrefsKeyPropertyModified, false);
			if (_isModified)
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.HelpBox("You've made some changes. \nPlease click on Save Changes button to apply those modifications.", 
				                        MessageType.Warning, 
				                        wide: true);
				GUI.color = Color.red;
				if (GUILayout.Button(m_saveChangesContent, GUILayout.ExpandWidth(false), GUILayout.MinHeight(38f)))
					OnPressingSave();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUI.color = _oldColor;
		}


		private void OnPressingSave()
		{
			((ReplayKitSettings)target).Rebuild();
		}
			   
		#endregion

		#region Style Methods

		public GUIStyle GetBoldLabelStyle(int _size, TextAnchor _alignement = TextAnchor.MiddleLeft)
		{
			GUIStyle _titleStyle 	= new GUIStyle(EditorStyles.boldLabel);
			_titleStyle.alignment	= _alignement;
			_titleStyle.fontSize	= _size;

			return _titleStyle;
		}

		public GUIStyle GetMenuButtonStyle()
		{
			GUIStyle _titleStyle 	= new GUIStyle(GUI.skin.button);
			_titleStyle.fontSize	= 14;

			return _titleStyle;
		}

		#endregion
	
	}
}
#endif