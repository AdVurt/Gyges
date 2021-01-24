using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.MeshTilemapBuilder {

    [CustomEditor(typeof(MeshTilemap))]
    public class MeshTilemapEditor : Editor {

        public enum Tool {
            Select,
            Paint,
            Erase
        }

        private readonly static Vector2 _maxScrollBounds = new Vector2(269f, 267f);
        private const float _visualGridSquareSize = 16f;
        private const int _visualGridSize = 32;

        private MeshTilemap _target;
        private SerializedProperty _renderCamera;
        private SerializedProperty _ruleTile;
        private SerializedProperty _positions;
        private SerializedProperty _tiles;


        private Vector2 _scrollOffset = Vector2.Lerp(Vector2.zero, _maxScrollBounds, 0.5f);
        private int _selected = -1;
        private Tool _tool;

        void OnEnable() {
            _target = (MeshTilemap)target;
            _renderCamera = serializedObject.FindProperty("renderCamera");
            _ruleTile = serializedObject.FindProperty("ruleTile");
            _positions = serializedObject.FindProperty("positions");
            _tiles = serializedObject.FindProperty("tiles");
            _scrollOffset = Vector2.Lerp(Vector2.zero, _maxScrollBounds, 0.5f);
            _selected = -1;
            _tool = Tool.Select;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_renderCamera);
            EditorGUILayout.PropertyField(_ruleTile);
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (_ruleTile.objectReferenceValue == null) {
                return;
            }

            GUILayout.BeginHorizontal();

            // Draw the visual grid editor.
            _scrollOffset = EditorGUILayout.BeginScrollView(_scrollOffset, GUILayout.Width(250f), GUILayout.Height(250f));
            DrawVisualEditor();
            EditorGUILayout.EndScrollView();


            // Tools should be in this vertical.
            GUILayoutOption toolWidth = GUILayout.Width(Screen.width - 289f);
            GUILayout.BeginVertical(toolWidth);

            // Tool selection
            //GUILayout.BeginHorizontal();
            using ( EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(_tool == Tool.Select))
                if (GUILayout.Button("Select")) {
                    _tool = Tool.Select;
                    _selected = -1;
                }
            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(_tool == Tool.Paint))
                if (GUILayout.Button("Paint")) {
                    _tool = Tool.Paint;
                    _selected = -1;
                }
            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(_tool == Tool.Erase))
                if (GUILayout.Button("Erase")) {
                    _tool = Tool.Erase;
                    _selected = -1;
                }
            //GUILayout.EndHorizontal();

            // Fields
            if (_tool == Tool.Select && _selected > -1) {

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Tile Mode", toolWidth);
                SerializedProperty modeProperty = _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("mode");
                EditorGUILayout.PropertyField(modeProperty, GUIContent.none, toolWidth);
                if (EditorGUI.EndChangeCheck()) {
                    if ((MeshTile.TileMode)modeProperty.enumValueIndex == MeshTile.TileMode.Override) {
                        _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMesh").objectReferenceValue = _target.ruleTile.defaultMesh;
                        SerializedProperty materialsProp = _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMaterials");

                        materialsProp.arraySize = _target.ruleTile.defaultMaterials.Length;
                        for (int i = 0; i < _target.ruleTile.defaultMaterials.Length; i++) {
                            materialsProp.GetArrayElementAtIndex(i).objectReferenceValue = _target.ruleTile.defaultMaterials[i];
                        }

                    }
                    else {
                        _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMesh").objectReferenceValue = null;
                        _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMaterials").arraySize = 0;
                    }
                }


                EditorGUILayout.Space();
                
                if (modeProperty.enumValueIndex == (int)MeshTile.TileMode.Override) {
                    EditorGUILayout.LabelField("Mesh", toolWidth);
                    SerializedProperty meshProperty = _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMesh");
                    EditorGUILayout.PropertyField(meshProperty, GUIContent.none, toolWidth);

                    EditorGUILayout.Space();

                    Mesh overrideMesh = (Mesh)meshProperty.objectReferenceValue;
                    EditorGUILayout.LabelField($"Material{((overrideMesh ?? _target.ruleTile.defaultMesh).subMeshCount > 1 ? "s" : "")}", toolWidth);
                    if (overrideMesh != null) {
                        SerializedProperty materialsProperty = _tiles.GetArrayElementAtIndex(_selected).FindPropertyRelative("overrideMaterials");
                        if (materialsProperty.arraySize != overrideMesh.subMeshCount)
                            materialsProperty.arraySize = overrideMesh.subMeshCount;

                        for (int i = 0; i < overrideMesh.subMeshCount; i++) {
                            EditorGUILayout.PropertyField(materialsProperty.GetArrayElementAtIndex(i), GUIContent.none);
                        }
                    }
                }
                else {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.LabelField("Mesh", toolWidth);
                    EditorGUILayout.ObjectField(GUIContent.none, _target.ruleTile.defaultMesh, typeof(Mesh), false, toolWidth);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField($"Material{(_target.ruleTile.defaultMesh.subMeshCount > 1 ? "s" : "")}", toolWidth);
                    for (int i = 0; i < _target.ruleTile.defaultMaterials.Length; i++) {
                        EditorGUILayout.ObjectField(GUIContent.none, _target.ruleTile.defaultMaterials[i], typeof(Material), false, toolWidth);
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            else {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.LabelField("Tile Mode", toolWidth);
                EditorGUILayout.EnumPopup(MeshTile.TileMode.Dynamic, toolWidth);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Mesh", toolWidth);
                EditorGUILayout.ObjectField(GUIContent.none, _target.ruleTile.defaultMesh, typeof(Mesh), false, toolWidth);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Material{(_target.ruleTile.defaultMesh.subMeshCount > 1 ? "s" : "")}", toolWidth);
                for (int i = 0; i < _target.ruleTile.defaultMaterials.Length; i++) {
                    EditorGUILayout.ObjectField(GUIContent.none, _target.ruleTile.defaultMaterials[i], typeof(Material), false, toolWidth);
                }

                EditorGUI.EndDisabledGroup();
            }


            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (serializedObject.ApplyModifiedProperties())
                _target.SyncUniquePositions();
        }


        /// <summary>
        /// Draws the visual "grid" editor.
        /// </summary>
        private void DrawVisualEditor() {
            Color bgColour = Color.Lerp( EditorGUIUtility.isProSkin ? new Color32(56,56,56,255) : new Color32(194,194,194,255),
                Color.black, 0.1f);

            Rect subRect = EditorGUILayout.GetControlRect(GUILayout.Width(_visualGridSquareSize * _visualGridSize), GUILayout.Height(_visualGridSquareSize * _visualGridSize));
            EditorGUI.DrawRect(subRect, bgColour);

            
            Vector2 centralPos = subRect.center - ((Vector2.up + Vector2.right) * _visualGridSquareSize);

            for (int i = 0; i < _target.positions.Length; i++) {

                Vector2Int gridItem = _target.positions[i];

                Rect visPos = new Rect(centralPos + ((Vector2.down * gridItem.y) + (Vector2.right * gridItem.x)) * _visualGridSquareSize,
                    new Vector2(_visualGridSquareSize, _visualGridSquareSize));
                DrawGridItem(i, visPos);

            }

            if (Event.current.type == EventType.MouseDown && subRect.Contains(Event.current.mousePosition)) {
                switch (_tool) {
                    case Tool.Select:
                        _selected = -1;
                        break;

                    case Tool.Paint:
                        Vector2Int mousePos = Vector2Int.FloorToInt((Event.current.mousePosition - centralPos) / _visualGridSquareSize) * (Vector2Int.right + Vector2Int.down);
                        GenerateItem(mousePos);
                        break;
                }
                
                Event.current.Use();
            }

            Handles.color = new Color(0f, 0f, 0f, 0.5f);

            for (int i = 0; i < _visualGridSize; i++) {
                //Horizontal line
                Handles.DrawLine(new Vector2(subRect.xMin, subRect.y + _visualGridSquareSize * i),
                                 new Vector2(subRect.xMax, subRect.y + _visualGridSquareSize * i));

                //Vertical line
                Handles.DrawLine(new Vector2(subRect.x + _visualGridSquareSize * i, subRect.yMin),
                                 new Vector2(subRect.x + _visualGridSquareSize * i, subRect.yMax));
            }

            //Draw "centre" icon
            EditorGUI.DrawRect(new Rect(centralPos.x + 4, centralPos.y + 4, _visualGridSquareSize - 8, _visualGridSquareSize - 8), new Color(0.5f, 1f, 0f, 0.5f));
        }


        /// <summary>
        /// Draws an individual grid item in the grid.
        /// </summary>
        /// <param name="id">The array ID of the item.</param>
        /// <param name="visualPosition">The rect position of the item in theh GUI.</param>
        private void DrawGridItem(int id, Rect visualPosition) {

            if (Event.current.type == EventType.MouseDown && visualPosition.Contains(Event.current.mousePosition)) {

                switch (_tool) {
                    case Tool.Select:
                        _selected = id;
                        break;

                    case Tool.Erase:
                        _positions.DeleteArrayElementAtIndex(id);
                        _tiles.DeleteArrayElementAtIndex(id);
                        serializedObject.ApplyModifiedProperties();
                        _target.SyncUniquePositions();
                        break;
                }

                Event.current.Use();
            }

            EditorGUI.DrawRect(visualPosition, _selected == id ? Color.yellow : Color.white);

        }

        private void GenerateItem(Vector2Int position) {

            if (_target.uniquePositions.Contains(position))
                return;

            _positions.arraySize++;
            _tiles.arraySize = _positions.arraySize;            
            _positions.GetArrayElementAtIndex(_positions.arraySize - 1).vector2IntValue = position;

            SerializedProperty newTile = _tiles.GetArrayElementAtIndex(_tiles.arraySize - 1);
            newTile.FindPropertyRelative("mode").enumValueIndex = (int)MeshTile.TileMode.Dynamic;
            newTile.FindPropertyRelative("overrideMesh").objectReferenceValue = null;
            newTile.FindPropertyRelative("overrideMaterials").arraySize = 0;

            serializedObject.ApplyModifiedProperties();

            _target.SyncUniquePositions();
        }
    }

}