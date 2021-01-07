using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(TimeBasedEnemyActions)), CanEditMultipleObjects]
    public class TimeBasedEnemyActionInspector : Editor {

        private SerializedProperty _timeMultiplier;
        private SerializedProperty _actions;
        private SerializedProperty _selfDestructWhenDone;
        private SerializedProperty _onFinished;

        void OnEnable() {
            _timeMultiplier = serializedObject.FindProperty("timeMultiplier");
            _actions = serializedObject.FindProperty("_actions");
            _selfDestructWhenDone = serializedObject.FindProperty("selfDestructWhenDone");
            _onFinished = serializedObject.FindProperty("onFinished");
        }


        public override void OnInspectorGUI() {

            serializedObject.Update();

            if (EditorApplication.isPlaying) {
                EditorGUILayout.LabelField("Current Stats", EditorStyles.boldLabel);
                using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(true)) {
                    TimeBasedEnemyActions t = (TimeBasedEnemyActions)target;
                    EditorGUILayout.Vector2Field("Velocity", t.GetVelocity());
                    EditorGUILayout.FloatField("Rotation Speed", t.GetRotationSpeed());
                }
            }

            EditorGUILayout.PropertyField(_timeMultiplier);

            if (AllHaveSameNumberOfActions())
                EditorGUILayout.PropertyField(_actions);
            else
                EditorGUILayout.HelpBox("Array multi-editing not supported when array sizes differ.",MessageType.Info);

            EditorGUILayout.PropertyField(_selfDestructWhenDone, new GUIContent("Die when Complete"));
            EditorGUILayout.PropertyField(_onFinished);

            serializedObject.ApplyModifiedProperties();
        }

        private bool AllHaveSameNumberOfActions() {
            if (!serializedObject.isEditingMultipleObjects)
                return true;

            int actionCount = ((TimeBasedEnemyActions)serializedObject.targetObjects[0]).GetActionCount();
            for (int i = 1; i < serializedObject.targetObjects.Length; i++) {
                if (((TimeBasedEnemyActions)serializedObject.targetObjects[i]).GetActionCount() != actionCount)
                    return false;
            }

            return true;
        }
    }
}
