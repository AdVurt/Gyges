using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(Enemy)), CanEditMultipleObjects]
    public class EnemyInspector : Editor {

        private SerializedProperty _script;
        private SerializedProperty _startingHealth;
        private SerializedProperty _bounty;
        private SerializedProperty _lootPrefabs;
        private SerializedProperty _lootFormation;
        private SerializedProperty _dissolveSpeed;
        private SerializedProperty _additionalRenderers;


        void OnEnable() {
            _script = serializedObject.FindProperty("m_Script");
            _startingHealth = serializedObject.FindProperty("startingHealth");
            _bounty = serializedObject.FindProperty("bounty");
            _lootPrefabs = serializedObject.FindProperty("lootPrefabs");
            _lootFormation = serializedObject.FindProperty("lootFormation");
            _dissolveSpeed = serializedObject.FindProperty("_dissolveSpeed");
            _additionalRenderers = serializedObject.FindProperty("_additionalRenderers");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            bool GUIen = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = GUIen;

            EditorGUILayout.PropertyField(_startingHealth);
            EditorGUILayout.PropertyField(_bounty);
            EditorGUILayout.PropertyField(_lootPrefabs);

            if (serializedObject.isEditingMultipleObjects) {
                EditorGUILayout.HelpBox("Loot formation editing not supported in multi-object editing.", MessageType.Info);
            }
            else if (_lootPrefabs.arraySize > 0) {
                EditorGUILayout.PropertyField(_lootFormation);
            }

            EditorGUILayout.PropertyField(_dissolveSpeed);
            EditorGUILayout.PropertyField(_additionalRenderers);

            serializedObject.ApplyModifiedProperties();
        }

    }

}