using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(SingleEnemyAction)), CanEditMultipleObjects]
    public class SingleEnemyActionInspector : Editor {

        private SerializedProperty _action;
        private SerializedProperty _selfDestructWhenDone;
        private SerializedProperty _onFinished;

        void OnEnable() {
            SerializedProperty actionArray = serializedObject.FindProperty("_actions");
            if (actionArray.arraySize != 1) {
                serializedObject.Update();
                actionArray.arraySize = 1;
                serializedObject.ApplyModifiedProperties();
            }

            _action = serializedObject.FindProperty("_actions").GetArrayElementAtIndex(0);
            _selfDestructWhenDone = serializedObject.FindProperty("selfDestructWhenDone");
            _onFinished = serializedObject.FindProperty("onFinished");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            if (EditorApplication.isPlaying) {
                EditorGUILayout.LabelField("Current Stats", EditorStyles.boldLabel);
                using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(true)) {
                    SingleEnemyAction t = (SingleEnemyAction)target;
                    EditorGUILayout.Vector2Field("Velocity",t.GetVelocity());
                    EditorGUILayout.FloatField("Rotation Speed", t.GetRotationSpeed());
                }
            }

            EditorGUILayout.PropertyField(_action);
            EditorGUILayout.PropertyField(_selfDestructWhenDone, new GUIContent("Die when Complete"));
            EditorGUILayout.PropertyField(_onFinished);

            serializedObject.ApplyModifiedProperties();
        }
    }

}