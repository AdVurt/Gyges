using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Utility;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            bool GUIen = GUI.enabled;
            GUI.enabled = !EditorApplication.isPlaying && ((ReadOnlyAttribute)attribute).editableInEditMode;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = GUIen;
        }
    }
}