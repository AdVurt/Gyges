using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(Generator)), CanEditMultipleObjects]
    public class GeneratorInspector : ShipPartInspector {

        private SerializedProperty _powerGeneration;

        new void OnEnable() {
            base.OnEnable();

            _powerGeneration = serializedObject.FindProperty("powerGeneration");
        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();
            EditorGUILayout.LabelField("Generator-Specific Data", EditorStyles.boldLabel);
            serializedObject.Update();

            EditorGUILayout.PropertyField(_powerGeneration);

            serializedObject.ApplyModifiedProperties();
        }

    }

}