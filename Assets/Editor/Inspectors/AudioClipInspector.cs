using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(AudioClip))]
    public class AudioClipInspector : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            bool en = GUI.enabled;
            GUI.enabled = true;
            EditorGUILayout.Space();
            if (EditorApplication.isPlaying && GUILayout.Button("Play")) {
                _Play((AudioClip)target);
            }
            GUI.enabled = en;
        }

        [OnOpenAsset(1)]
        public static bool Play(int instanceID, int line) {

            Object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj.GetType() != typeof(AudioClip))
                return false;

            if (EditorApplication.isPlaying && obj != null) {
                _Play((AudioClip)obj);
                return true;
            }
            return false;
        }

        private static void _Play(AudioClip clip) {
            Game.MusicManager.Play(clip);
        }

    }

}