using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(Shield)), CanEditMultipleObjects]
    public class ShieldInspector : ShipPartInspector {

        private Shield _target;

        private SerializedProperty _shieldMax;
        private SerializedProperty _regenRate;
        private SerializedProperty _costPerSecond;
        private SerializedProperty _powerThreshold;
        private SerializedProperty _regenDelay;

        new void OnEnable() {
            base.OnEnable();
            _target = (Shield)target;

            _shieldMax = serializedObject.FindProperty("shieldMax");
            _regenRate = serializedObject.FindProperty("regenRate");
            _costPerSecond = serializedObject.FindProperty("costPerSecond");
            _powerThreshold = serializedObject.FindProperty("powerThreshold");
            _regenDelay = serializedObject.FindProperty("regenDelay");
        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();
            EditorGUILayout.LabelField("Shield-Specific Data", EditorStyles.boldLabel);
            serializedObject.Update();

            EditorGUILayout.PropertyField(_shieldMax);
            EditorGUILayout.Space();

            using (EditorGUI.DisabledGroupScope s = new EditorGUI.DisabledGroupScope(_shieldMax.floatValue == 0f)) {
                EditorGUILayout.PropertyField(_regenRate);
                EditorGUILayout.PropertyField(_costPerSecond);
                EditorGUILayout.PropertyField(_powerThreshold);
                EditorGUILayout.PropertyField(_regenDelay);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}