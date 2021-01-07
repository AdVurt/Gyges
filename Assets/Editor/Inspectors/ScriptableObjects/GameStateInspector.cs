using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(GameState))]
    public class GameStateInspector : Editor {

        private GameState _target;
        private SerializedProperty _script;
        private SerializedProperty _currentLevel;
        private SerializedProperty _availableLevels;
        private SerializedProperty _loadouts;
        private SerializedProperty _totalPoints;
        private SerializedProperty _shopStock;


        void OnEnable() {
            _target = (GameState)target;

            _script = serializedObject.FindProperty("m_Script");
            _currentLevel = serializedObject.FindProperty("currentLevel");
            _availableLevels = serializedObject.FindProperty("availableLevels");
            _loadouts = serializedObject.FindProperty("loadouts");
            _totalPoints = serializedObject.FindProperty("totalPoints");
            _shopStock = serializedObject.FindProperty("shopStock");
        }


        public override void OnInspectorGUI() {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;

            serializedObject.Update();

            EditorGUILayout.PropertyField(_currentLevel);
            EditorGUILayout.PropertyField(_availableLevels);
            EditorGUILayout.PropertyField(_loadouts);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_shopStock);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_totalPoints);
            for (int i = 0; i < _loadouts.arraySize; i++) {
                EditorGUILayout.LabelField($"Loadout {i+1} Funds", (_totalPoints.intValue - _target.loadouts[i].GetTotalValue()).ToString());
            }

            if (_target.name == "Global State") {
                EditorGUILayout.Space();
                if (GUILayout.Button("Reset to Default")) {
                    _target.SetState(null, null, 0, null);
                    EditorUtility.SetDirty(_target);
                }
                GameState copyFrom = (GameState)EditorGUILayout.ObjectField(new GUIContent("Copy From") ,null, typeof(GameState), false);
                if (copyFrom != null) {
                    _target.CopyFrom(copyFrom);
                    EditorUtility.SetDirty(_target);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}