using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;
using System.Linq;
using System;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(Hull)), CanEditMultipleObjects]
    public class HullInspector : ShipPartInspector {

        //These are for the mesh preview
        private bool _drawPreview = false;
        private static Vector2 _previewRotation = Vector2.zero;
        private PreviewRenderUtility _previewRenderer;

        private SerializedProperty _startingHull;
        private SerializedProperty _shipModel;
        private SerializedProperty _modelMaterials;
        private SerializedProperty _trailPositions;

        public new void OnEnable() {

            base.OnEnable();
            _startingHull = serializedObject.FindProperty("startingHull");
            _shipModel = serializedObject.FindProperty("shipModel");
            _modelMaterials = serializedObject.FindProperty("modelMaterials");
            _trailPositions = serializedObject.FindProperty("trailPositions");
            InitialisePreviewRenderer();
            
        }

        void OnDisable() {
            _previewRenderer.Cleanup();
        }

        /// <summary>
        /// Initialises the preview renderer.
        /// </summary>
        public void InitialisePreviewRenderer() {

            if (_previewRenderer == null) {
                _previewRenderer = new PreviewRenderUtility();
            }
            _previewRenderer.camera.transform.position = new Vector3(0f, 0f, -10f);
            _previewRenderer.camera.farClipPlane = 20f;

            _previewRenderer.lights[0].transform.rotation = Quaternion.Euler(15f,33.9f,12.8f);

        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();

            serializedObject.Update();
            EditorGUILayout.LabelField("Hull-Specific Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_startingHull);

            EditorGUILayout.PropertyField(_shipModel);

            // Figure out whether all selected hulls have the same (non-null) ship model
            Mesh firstMesh = ((Hull)targets[0]).shipModel;
            bool allTheSame = firstMesh != null;
            if (allTheSame && !serializedObject.isEditingMultipleObjects) {
                for (int i = 1; i < targets.Length; i++) {
                    if (((Hull)targets[0]).shipModel != allTheSame) {
                        allTheSame = false;
                        break;
                    }
                }
            }

            Material firstMaterial = null;
            bool drawModelPreview = false;
            // If all selected hulls share a ship model, process the material field(s)
            if (allTheSame) {
                drawModelPreview = true;

                EditorGUI.indentLevel++;

                for (int i = 0; i < firstMesh.subMeshCount; i++) {
                    if (_modelMaterials.arraySize <= i)
                        _modelMaterials.arraySize = i + 1;

                    SerializedProperty material = _modelMaterials.GetArrayElementAtIndex(i);
                    if (material.hasMultipleDifferentValues) {
                        drawModelPreview = false;
                    }
                    else if (i == 0) {
                        firstMaterial = (Material)material.objectReferenceValue;
                    }
                    EditorGUILayout.PropertyField(material, new GUIContent($"Material {i}"));

                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_trailPositions);

            serializedObject.ApplyModifiedProperties();

            //Draw the preview model
            _drawPreview = drawModelPreview;
        }


        public override bool HasPreviewGUI() => _drawPreview;

        public override void OnPreviewGUI(Rect r, GUIStyle background) {

            _previewRenderer.BeginPreview(r, background);
            Mesh mesh = (Mesh)_shipModel.objectReferenceValue;

            _previewRotation = Utility.GygesEditorUtility.Drag2D(_previewRotation, r);

            for (int i = 0; i < mesh.subMeshCount; i++) {
                _previewRenderer.DrawMesh(mesh, Vector3.zero, Quaternion.Euler(_previewRotation.y, _previewRotation.x, 0f), (Material)_modelMaterials.GetArrayElementAtIndex(i).objectReferenceValue, i);
            }
            _previewRenderer.camera.Render();
            _previewRenderer.EndAndDrawPreview(r);
        }



        

    }

}