using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Gyges.MeshTilemapBuilder {

    [CustomEditor(typeof(MeshRuleTile), true)]
    public class MeshRuleTileEditor : Editor {

        private const string _xIconString = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";
        private const string _arrow0 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
        private const string _arrow1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
        private const string _arrow2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
        private const string _arrow3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
        private const string _arrow5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
        private const string _arrow6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
        private const string _arrow7 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
        private const string _arrow8 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
        private const string _fixed = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";

        private static Texture2D[] _arrows;
        private static Texture2D _selfIcon;
        private const float _labelWidth = 80f;

        private PreviewRenderUtility _renderUtility;

        /// <summary>
        /// Array of arrow textures used for marking positions for rule matches.
        /// </summary>
        public static Texture2D[] Arrows {
            get {
                if (_arrows == null) {
                    _arrows = new Texture2D[10];
                    _arrows[0] = Base64ToTexture(_arrow0);
                    _arrows[1] = Base64ToTexture(_arrow1);
                    _arrows[2] = Base64ToTexture(_arrow2);
                    _arrows[3] = Base64ToTexture(_arrow3);
                    _arrows[5] = Base64ToTexture(_arrow5);
                    _arrows[6] = Base64ToTexture(_arrow6);
                    _arrows[7] = Base64ToTexture(_arrow7);
                    _arrows[8] = Base64ToTexture(_arrow8);
                    _arrows[9] = Base64ToTexture(_xIconString);
                }
                return _arrows;
            }
        }

        public static Texture2D SelfIcon {
            get {
                if (_selfIcon == null)
                    _selfIcon = Base64ToTexture(_fixed);
                return _selfIcon;
            }
        }

        private ReorderableList _ruleLines;
        private MeshRuleTile _target;
        private SerializedProperty _defaultMesh;
        private SerializedProperty _defaultMaterials;

        private void OnEnable() {
            _target = (MeshRuleTile)target;

            _defaultMesh = serializedObject.FindProperty("defaultMesh");
            _defaultMaterials = serializedObject.FindProperty("defaultMaterials");

            _ruleLines = new ReorderableList(_target?.lines, typeof(MeshRule), true, true, true, true) {
                drawHeaderCallback = OnDrawLineHeader,
                drawElementCallback = OnDrawLineElement,
                elementHeightCallback = GetLineElementHeight,
                onChangedCallback = OnLineListUpdated,
                onAddCallback = OnLineListAddElement
            };
        }

        void OnDisable() {
            _renderUtility?.Cleanup();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_defaultMesh);
            EditorGUILayout.PropertyField(_defaultMaterials);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginChangeCheck();
            _ruleLines.DoLayoutList();

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Reset Textures")) {
                _arrows = null;
                _selfIcon = null;
            }

            
        }

        private static Texture2D Base64ToTexture(string base64) {
            Texture2D t = new Texture2D(1, 1) {
                hideFlags = HideFlags.HideAndDontSave
            };
            t.LoadImage(System.Convert.FromBase64String(base64));
            return t;
        }

        #region --- ReorderableList callbacks

        private void OnDrawLineHeader(Rect rect) { GUI.Label(rect, "Mesh Tiling Rules"); }

        /// <summary>
        /// This is the main script used to draw the rule line in the inspector.
        /// </summary>
        private void OnDrawLineElement(Rect rect, int index, bool isActive, bool isFocused) {

            MeshRule rule = _target.lines[index];

            Rect preview = new Rect(rect.x, rect.y + 5, 50, 50);
            Rect r = new Rect(rect.x + 54, rect.y, rect.width - 108, rect.height);

            if (_renderUtility == null)
                _renderUtility = new PreviewRenderUtility();

            _renderUtility.camera.orthographic = true;
            _renderUtility.camera.orthographicSize = 0.5f;
            _renderUtility.camera.transform.position = new Vector3(0f, 0f, -10f);
            _renderUtility.camera.farClipPlane = 20f;
            _renderUtility.lights[0].transform.rotation = Quaternion.Euler(15f, 33.9f, 12.8f);

            _renderUtility.BeginPreview(preview, GUIStyle.none);
            if (rule.mesh != null && _defaultMaterials.arraySize >= rule.mesh.subMeshCount) {
                for (int i = 0; i < rule.mesh.subMeshCount; i++) {
                    SerializedProperty materialReference = _defaultMaterials.GetArrayElementAtIndex(i);
                    if (materialReference.objectReferenceValue != null)
                        _renderUtility.DrawMesh(rule.mesh, Vector3.zero, Quaternion.identity, (Material)materialReference.objectReferenceValue, i);
                }
            }
            _renderUtility.camera.Render();
            _renderUtility.EndAndDrawPreview(preview);
            
            
            rule.mesh = (Mesh)EditorGUI.ObjectField(r, GUIContent.none , rule.mesh, typeof(Mesh), false);


            int squareWidth = 16;

            r.x = r.xMax + (rect.width - r.width - 54)/2;
            r.width = squareWidth * 3;
            r.x -= squareWidth * 1.5f - 2;
            r.height = squareWidth * 3;
            r.y = rect.y + (rect.height - squareWidth*3) / 2;

            //Direction grid
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);

            for (int y = 0; y < 4; y++) {
                float top = r.yMin + y * squareWidth;
                Handles.DrawLine(new Vector3(r.xMin, top), new Vector3(r.xMax, top));
            }
            for (int x = 0; x < 4; x++) {
                float left = r.xMin + x * squareWidth;
                Handles.DrawLine(new Vector3(left, r.yMin), new Vector3(left, r.yMax));
            }
            Handles.color = Color.white;

            if (rule.relations.Length != 9) {
                rule.ResetRelations();
                GUI.changed = true;
            }
            MeshRuleRelation[] relations = rule.relations;

            int relationIndex = -1;
            for (int i = 0; i < 9; i++) {
                Texture2D tex = null;

                Rect squareRect = new Rect(r.x + ((i % 3) * squareWidth), r.y + (i / 3) * squareWidth, squareWidth, squareWidth);

                if (i == 4) {
                    tex = SelfIcon;
                }
                else {
                    relationIndex++;

                    //If this has been clicked by the left mouse button
                    if (Event.current.type == EventType.MouseDown && squareRect.Contains(Event.current.mousePosition)) {
                        relations[i].check = (MeshRuleRelation.CheckType)(((int)relations[i].check + 1) % 3);
                        GUI.changed = true;
                    }

                    switch (relations[i].check) {

                        case MeshRuleRelation.CheckType.Include:
                            tex = Arrows[i];
                            break;

                        case MeshRuleRelation.CheckType.Exclude:
                            tex = Arrows[9];
                            break;

                        default:
                            break;
                    }
                }

                if (tex != null)
                    GUI.DrawTexture(squareRect, tex);
            }

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                Event.current.Use();

        }

        private float GetLineElementHeight(int index) => 60f;

        private void OnLineListUpdated(ReorderableList list) {
            HashSet<int> usedIdSet = new HashSet<int>();
            foreach (MeshRule rule in _target.lines) {
                while (usedIdSet.Contains(rule.id))
                    rule.id++;
                usedIdSet.Add(rule.id);
            }
            EditorUtility.SetDirty(target);
        }

        private void OnLineListAddElement(ReorderableList list) {
            MeshRule rule = new MeshRule {
                mesh = _target.defaultMesh
            };
            _target.lines.Add(rule);
            EditorUtility.SetDirty(target);
        }

        #endregion
    }
}