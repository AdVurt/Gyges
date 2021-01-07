using System;
using UnityEngine;
using UnityEditor;
using Gyges.Utility;

namespace Gyges.CustomEditors {

    public abstract class ReferenceDrawer<T> : PropertyDrawer {

        private readonly string[] _popupOptions = { "Use Constant", "Use Variable" };
        private GUIStyle _popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            if (_popupStyle == null) {
                _popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) {
                    imagePosition = ImagePosition.ImageOnly
                };
            }

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            SerializedProperty useConstant = property.FindPropertyRelative("useConstant");
            SerializedProperty constantValue = property.FindPropertyRelative("constantValue");
            SerializedProperty variable = property.FindPropertyRelative("variable");

            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _popupStyle.margin.top;
            buttonRect.width = _popupStyle.fixedWidth + _popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            useConstant.boolValue = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, _popupOptions, _popupStyle) == 0;
            if (useConstant.boolValue) {
                EditorGUI.PropertyField(position, constantValue, GUIContent.none);
            }
            else {
                EditorGUI.ObjectField(position, variable, typeof(T), GUIContent.none);
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = oldIndent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : ReferenceDrawer<BoolVariable> { }
    [CustomPropertyDrawer(typeof(ColourReference))]
    public class ColourReferenceDrawer : ReferenceDrawer<ColourVariable> { }
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : ReferenceDrawer<FloatVariable> { }
    [CustomPropertyDrawer(typeof(IntReference))]
    public class IntReferenceDrawer : ReferenceDrawer<IntVariable> { }
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferenceDrawer : ReferenceDrawer<StringVariable> { }
    [CustomPropertyDrawer(typeof(Vector2Reference))]
    public class Vector2ReferenceDrawer : ReferenceDrawer<Vector2Variable> { }
    [CustomPropertyDrawer(typeof(Vector3Reference))]
    public class Vector3ReferenceDrawer : ReferenceDrawer<Vector3Variable> { }
}