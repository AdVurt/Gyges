using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(WaveGroup)), CanEditMultipleObjects]
    public class WaveGroupInspector : Editor {

        private WaveGroup _target;

        void OnEnable() {
            _target = (WaveGroup)target;
        }


        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (!EditorApplication.isPlaying)
                return;

            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(true)) {

                EditorGUILayout.Space();

                foreach (Transform t in _target.transform) {
                    if (t == _target.transform)
                        continue;

                    GUILayout.Label(t.gameObject.name);

                    foreach (IWaveObject obj in t.GetComponents<IWaveObject>()) {
                        EditorGUILayout.Toggle("Out of bounds",obj.IsOutOfBounds());
                    }

                    


                }

            }

        }


    }

}