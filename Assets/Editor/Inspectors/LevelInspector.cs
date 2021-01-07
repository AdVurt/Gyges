using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(Level)), CanEditMultipleObjects]
    public class LevelInspector : Editor {

        SerializedProperty _scene;
        SerializedProperty _inGameName;
        SerializedProperty _startingMusic;

        void OnEnable() {
            _scene = serializedObject.FindProperty("scene");
            _inGameName = serializedObject.FindProperty("inGameName");
            _startingMusic = serializedObject.FindProperty("startingMusic");
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(_scene);
            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(!(targets.Length == 1 && ((Level)target).scene != null))) {
                if (GUILayout.Button("Open Scene")) {

                    foreach (Object t in targets) {
                        Level level = (Level)t;
                        if (level.scene != null && level.scene.scene != null)
                            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(level.scene.scene));
                    }

                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_inGameName);
            EditorGUILayout.PropertyField(_startingMusic);

            serializedObject.ApplyModifiedProperties();

        }

    }

}