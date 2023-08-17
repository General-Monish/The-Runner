using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

namespace VoxelBusters.ReplayKit.Internal
{
    [CustomPropertyDrawer(typeof(CustomBitRateSetting))]
    public class CustomBitRateSettingDrawer : PropertyDrawer
    {
        #region Drawer Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent(label.text+"[Optional]"));
                SerializedProperty bitRatesFlag = property.FindPropertyRelative("m_allowCustomBitrates");
                bitRatesFlag.boolValue = EditorGUILayout.Toggle(bitRatesFlag.boolValue);
                EditorGUILayout.EndHorizontal();

                if (bitRatesFlag.boolValue)
                {
                    SerializedProperty bitRateFactor = property.FindPropertyRelative("m_bitrateFactor");

                    position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                    EditorGUI.indentLevel++;
                    // Draw label
                    position = EditorGUI.PrefixLabel(position, new GUIContent("Bitrate Factor"));
                    EditorGUI.indentLevel--;

                    // Draw slider
                    bitRateFactor.floatValue = EditorGUI.Slider(position, bitRateFactor.floatValue, 0f, 1f);
                    float labelWidth = position.width;

                    // Move to next line
                    position.y          += EditorGUIUtility.singleLineHeight;

                    // Subtract the text field width thats drawn with slider
                    position.width      -= EditorGUIUtility.fieldWidth;

                    GUIStyle style = GUI.skin.label;
                    TextAnchor defaultAlignment = GUI.skin.label.alignment;
                    style.alignment = TextAnchor.UpperLeft; EditorGUI.LabelField(position, "Low Bitrate", style);
                    style.alignment = TextAnchor.UpperRight; EditorGUI.LabelField(position, "High Bitrate", style);
                    GUI.skin.label.alignment = defaultAlignment;
                }
                else
                {
                    EditorGUILayout.HelpBox("[Optional Setting] \n" +
                                      "Enable custom bitrates to set general recommended bitrates for smaller video sizes compared to default bitrates \n" +
                                      "(https://support.video.ibm.com/hc/en-us/articles/207852117-Internet-connection-and-recommended-encoding-settings)", MessageType.Info);
                }

            if (EditorGUI.EndChangeCheck())
            {
                SerializedObject _serializedObject = property.serializedObject;
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();

        }

        #endregion
    }
}
#endif