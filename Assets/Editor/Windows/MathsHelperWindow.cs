using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.CustomEditors {

    public class MathsHelperWindow : EditorWindow {

        private Vector2 _direction = Vector2.zero;
        private float _transformAmount = 0f;

        private string _displayedOutput = "";

        [MenuItem("Window/Gyges/Maths Helper")]
        public static void ShowWindow() {
            GetWindow<MathsHelperWindow>().titleContent = new GUIContent("Maths Helper");
        }


        void OnGUI() {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(200));
            _direction = EditorGUILayout.Vector2Field("Direction", _direction);
            _transformAmount = EditorGUILayout.FloatField("Manipulation", _transformAmount);

            EditorGUILayout.BeginHorizontal();
            Vector2 oldVal = _direction;
            if (GUILayout.Button("Add")) {
                _displayedOutput = $"({_direction.x},{_direction.y}) + {_transformAmount} = {_direction + (_direction.normalized * _transformAmount)}";
            }
            if (GUILayout.Button("Multiply")) {
                _displayedOutput = $"({_direction.x},{_direction.y}) x {_transformAmount} = {_direction * _transformAmount}";
            }
            if (GUILayout.Button("Rotate")) {
                Vector2 res = Quaternion.AngleAxis(_transformAmount, Vector3.forward) * _direction;
                _displayedOutput = $"({_direction.x},{_direction.y}) rotated by {_transformAmount} = ({res.x},{res.y})";
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUILayout.PrefixLabel("Output");
            EditorGUILayout.TextArea(_displayedOutput, GUILayout.Height(100f));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

    }

}