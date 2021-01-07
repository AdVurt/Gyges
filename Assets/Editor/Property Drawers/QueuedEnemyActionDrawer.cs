using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {
    [CustomPropertyDrawer(typeof(QueuedEnemyAction))]
    public class QueuedEnemyActionDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            float height;

            switch ((QueuedEnemyAction.ActionType)property.FindPropertyRelative("actionType").enumValueIndex) {
                case QueuedEnemyAction.ActionType.WaitForSeconds:
                    height = EditorGUIUtility.singleLineHeight * 2;
                    break;
                case QueuedEnemyAction.ActionType.SetVelocity:
                    height = EditorGUIUtility.singleLineHeight * 3;
                    break;
                case QueuedEnemyAction.ActionType.Loop:
                    height = EditorGUIUtility.singleLineHeight;
                    break;
                case QueuedEnemyAction.ActionType.SetRotationSpeed:
                    height = EditorGUIUtility.singleLineHeight * 3;
                    break;
                default:
                    throw new System.Exception("Unknown queued enemy action type.");
            }
            return height + 10;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            float yPos = position.y + 2;
            SerializedProperty _actionType = property.FindPropertyRelative("actionType");
            SerializedProperty _intValues = property.FindPropertyRelative("intValues");
            SerializedProperty _floatValues = property.FindPropertyRelative("floatValues");
            SerializedProperty _vector2Values = property.FindPropertyRelative("vector2Values");

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(new Rect(position.x, yPos, position.width / 2, EditorGUIUtility.singleLineHeight), _actionType, new GUIContent());
            yPos += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.indentLevel++;
            switch ((QueuedEnemyAction.ActionType)_actionType.enumValueIndex) {

                case QueuedEnemyAction.ActionType.WaitForSeconds:
                    if (_floatValues.arraySize != 1)
                        _floatValues.arraySize = 1;
                    EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight),
                        _floatValues.GetArrayElementAtIndex(0),new GUIContent("Seconds"));
                    break;

                case QueuedEnemyAction.ActionType.SetVelocity:
                    if (_vector2Values.arraySize != 1)
                        _vector2Values.arraySize = 1;
                    if (_floatValues.arraySize != 1)
                        _floatValues.arraySize = 1;
                    EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight),
                        _vector2Values.GetArrayElementAtIndex(0), new GUIContent("Velocity"));
                    yPos += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight),
                        _floatValues.GetArrayElementAtIndex(0), new GUIContent("Change time"));
                    break;

                case QueuedEnemyAction.ActionType.SetRotationSpeed:
                    if (_floatValues.arraySize != 2)
                        _floatValues.arraySize = 2;
                    EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight),
                        _floatValues.GetArrayElementAtIndex(0), new GUIContent("Rotation speed"));
                    yPos += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight),
                        _floatValues.GetArrayElementAtIndex(1), new GUIContent("Change time"));
                    break;

                case QueuedEnemyAction.ActionType.Loop:
                    break;

                default:
                    throw new System.Exception("Unknown queued enemy action type.");
            }


            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

    }
}