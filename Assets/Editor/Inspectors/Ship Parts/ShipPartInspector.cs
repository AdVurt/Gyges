using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.CustomEditors {

    public class ShipPartInspector : Editor {

        protected SerializedProperty _script;
        protected SerializedProperty _readyToPlay;
        protected SerializedProperty _baseCost;
        protected SerializedProperty _inGameUIName;

        public void OnEnable() {
            _script = serializedObject.FindProperty("m_Script");
            _readyToPlay = serializedObject.FindProperty("readyToPlay");
            _baseCost = serializedObject.FindProperty("baseCost");
            _inGameUIName = serializedObject.FindProperty("inGameUIName");
        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();
        }

        protected void DrawShipPartInspector() {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Standard Item Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_baseCost);
            EditorGUILayout.PropertyField(_inGameUIName);
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

    }

}