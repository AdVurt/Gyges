using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.CustomEditors {
    [CustomEditor(typeof(GameplaySettings))]
    public class GameplaySettingsInspector : Editor {

        GameplaySettings _target;

        private void OnEnable() {
            _target = (GameplaySettings)target;
        }

        public override void OnInspectorGUI() {

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            DrawDefaultInspector();
            EditorGUI.EndDisabledGroup();

            if (Application.isPlaying) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("For Gameplay Use", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                using (EditorGUI.ChangeCheckScope sc = new EditorGUI.ChangeCheckScope()) {
                    bool val = GUILayout.Toggle(_target.P1HUDOnRight, "P1HUD On Right");
                    if (sc.changed) {
                        _target.P1HUDOnRight = val;
                    }
                }

                using (EditorGUI.ChangeCheckScope sc = new EditorGUI.ChangeCheckScope()) {
                    bool val = GUILayout.Toggle(_target.StreamerMode, "Streamer Mode");
                    if (sc.changed) {
                        _target.StreamerMode = val;
                    }
                }
            }
        }

    }
}