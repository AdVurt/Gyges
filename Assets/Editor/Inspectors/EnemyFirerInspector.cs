using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(EnemyFirer))]
    public class EnemyFirerInspector : Editor {

        private SerializedProperty _fireType;
        private SerializedProperty _projectilePrefab;
        private SerializedProperty _delayBetweenShots;
        private SerializedProperty _projectileBehaviour;
        private SerializedProperty _projectileFormation;
        private SerializedProperty _direction;
        private SerializedProperty _directionArray;
        private SerializedProperty _useFormation;
        private SerializedProperty _speed;
        private SerializedProperty _originOffset;
        private SerializedProperty _damage;

        void OnEnable() {

            _fireType = serializedObject.FindProperty("_fireType");
            _projectilePrefab = serializedObject.FindProperty("_projectilePrefab");
            _delayBetweenShots = serializedObject.FindProperty("_delayBetweenShots");
            _projectileBehaviour = serializedObject.FindProperty("_projectileBehaviour");
            _direction = serializedObject.FindProperty("_direction");
            _directionArray = serializedObject.FindProperty("_directionArray");
            _speed = serializedObject.FindProperty("_speed");
            _projectileFormation = serializedObject.FindProperty("_projectileFormation");
            _useFormation = serializedObject.FindProperty("_useFormation");
            _originOffset = serializedObject.FindProperty("_originOffset");
            _damage = serializedObject.FindProperty("_damage");
        }



        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(_fireType);
            if (_fireType.enumValueIndex != 0) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Firing Logic", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_delayBetweenShots, new GUIContent("Reload Time"));
                EditorGUILayout.PropertyField(_originOffset, new GUIContent("Origin Offset"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Projectile Logic", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_projectilePrefab, new GUIContent("Prefab"));

                if (_projectilePrefab.objectReferenceValue != null) {

                    EditorGUILayout.PropertyField(_projectileBehaviour, new GUIContent("Behaviour"));
                    if (!_useFormation.boolValue) {
                        EditorGUILayout.PropertyField(_speed);
                    }
                    EditorGUILayout.PropertyField(_damage);
                    EnemyFirer.ProjectileBehaviour beh = (EnemyFirer.ProjectileBehaviour)_projectileBehaviour.enumValueIndex;

                    switch (beh) {
                        case EnemyFirer.ProjectileBehaviour.FlatDirection:
                        case EnemyFirer.ProjectileBehaviour.Homing:
                            EditorGUILayout.PropertyField(_direction, new GUIContent("Base Direction", "This will be normalised in game logic, don't worry about magnitude."));
                            break;
                        case EnemyFirer.ProjectileBehaviour.RandomDirectionFromArray:
                            EditorGUILayout.PropertyField(_directionArray, new GUIContent("Directions"));
                            break;
                        default:
                            break;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Formation Logic", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(_useFormation, new GUIContent("Use Formation?", "Whether or not to fire multiple projectiles at once, with different offsetting positions and rotations."));
                    if (_useFormation.boolValue) {
                        EditorGUILayout.PropertyField(_projectileFormation);
                    }

                }
            }


            serializedObject.ApplyModifiedProperties();
        }

    }
}