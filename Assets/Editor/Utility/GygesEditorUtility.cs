using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.CustomEditors.Utility {

    public static class GygesEditorUtility {

        /// <summary>
        /// Customised version of EditorGUIUtility.Drag2D. Enables dragging over the preview window.
        /// </summary>
        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position) {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(controlID)) {

                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f) {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID) {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID) {
                        scrollPosition -= current.delta * ((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;

            }

            return scrollPosition;
        }

    }

}