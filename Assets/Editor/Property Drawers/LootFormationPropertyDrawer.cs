using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(LootFormation))]
    public class LootFormationPropertyDrawer : PropertyDrawer {

        private const int _maximumLootCount = 20;

        private static string[] _popupText = new string[] { "Add Loot", "Remove Loot", "Duplicate Loot", "Reset Loot" };
        private static Color _backgroundColor = Color.black;
        private static GUIStyle _circleStyle = new GUIStyle();
        private static Texture2D _circleTex;

        public static float height = 200f + EditorGUIUtility.singleLineHeight * 2 + 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(pos, label.text);
            pos.y += EditorGUIUtility.singleLineHeight * 2 + 4;

            DrawVisualBox(new Rect(pos.x + pos.width / 2 - 100f, pos.y, 200f, 200f), property);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty locs = property.FindPropertyRelative("_locs");
            SerializedProperty selectedLoot = property.FindPropertyRelative("selectedLoot");

            pos.y -= EditorGUIUtility.singleLineHeight + 2;

            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(locs.arraySize == 0)) {
                if (GUI.Button(new Rect(pos.x + 5f, pos.y, (pos.width / 2 - 112f)/2, EditorGUIUtility.singleLineHeight),"-")) {
                    if (selectedLoot.intValue >= locs.arraySize - 1) {
                        selectedLoot.intValue = locs.arraySize - 2;
                    }
                    locs.arraySize--;
                    GUI.FocusControl("");
                }
            }

            using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(locs.arraySize == _maximumLootCount)) {
                if (GUI.Button(new Rect(pos.x + 5f + (pos.width / 2 - 112f) / 2, pos.y, (pos.width / 2 - 112f) / 2, EditorGUIUtility.singleLineHeight), "+")) {
                    selectedLoot.intValue = locs.arraySize;
                    locs.arraySize++;
                    SerializedProperty newLoc = locs.GetArrayElementAtIndex(locs.arraySize - 1);
                    newLoc.FindPropertyRelative("x").floatValue = 0f;
                    newLoc.FindPropertyRelative("y").floatValue = 0f;
                    newLoc.FindPropertyRelative("speed").vector2Value = LootLocation.DefaultSpeed;
                    newLoc.FindPropertyRelative("scale").floatValue = 1f;
                    newLoc.FindPropertyRelative("prefabToUse").intValue = 0;
                    GUI.FocusControl("");
                }
            }

            pos.y += EditorGUIUtility.singleLineHeight + 2;

            Rect buttonRectLeft = new Rect(pos.x + 2f, pos.y, (pos.width / 2 - 103f) / 2 - 2f, 18f);
            Rect buttonRectRight = new Rect(buttonRectLeft);
            buttonRectRight.x += buttonRectLeft.width + 2;

            for (int i = 0; i < _maximumLootCount; i++) {
                ref Rect rectToUse = ref (i < _maximumLootCount / 2) ? ref buttonRectLeft : ref buttonRectRight;
                Color guiCol = GUI.color;
                if (selectedLoot.intValue == i) {
                    GUI.color = Color.red;
                }
                using (EditorGUI.DisabledGroupScope sc = new EditorGUI.DisabledGroupScope(selectedLoot.intValue == i || locs.arraySize <= i )) {
                    if (GUI.Button(rectToUse, new GUIContent((i + 1).ToString()))) {
                        selectedLoot.intValue = i;
                        GUI.FocusControl("");
                    }
                }
                GUI.color = guiCol;
                rectToUse.y += rectToUse.height + 2;
            }


            //The currently selected location, for further editing.
            if (selectedLoot.intValue >= locs.arraySize) {
                selectedLoot.intValue = -1;
            }
            SerializedProperty selected = (selectedLoot.intValue == -1) ? null : locs.GetArrayElementAtIndex(selectedLoot.intValue);
            Rect rightSideRect = new Rect(pos.x + pos.width/2 + 102f, position.y + EditorGUIUtility.singleLineHeight + 2, pos.width - (pos.width / 2 + 102f), pos.height);


            // If nothing is selected, draw disabled and default values
            if (selected == null) {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUI.LabelField(rightSideRect, "Position");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "X");
                EditorGUI.FloatField(MiniRectField(rightSideRect), 0f);
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "Y");
                EditorGUI.FloatField(MiniRectField(rightSideRect), 0f);

                rightSideRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.LabelField(rightSideRect, "Speed");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "X");
                EditorGUI.FloatField(MiniRectField(rightSideRect), 0f);
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "Y");
                EditorGUI.FloatField(MiniRectField(rightSideRect), 0f);

                rightSideRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.LabelField(rightSideRect, "Scale");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.FloatField(rightSideRect, 1f);

                EditorGUI.EndDisabledGroup();
            }
            //If something is selected, draw the values
            else {
                SerializedProperty x = selected.FindPropertyRelative("x");
                SerializedProperty y = selected.FindPropertyRelative("y");
                SerializedProperty speed = selected.FindPropertyRelative("speed");
                SerializedProperty scale = selected.FindPropertyRelative("scale");

                EditorGUI.LabelField(rightSideRect, "Position");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "X");
                x.floatValue = EditorGUI.FloatField(MiniRectField(rightSideRect), x.floatValue);
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "Y");
                y.floatValue = EditorGUI.FloatField(MiniRectField(rightSideRect), y.floatValue);

                rightSideRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.LabelField(rightSideRect, "Speed");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                Vector2 speedVal = speed.vector2Value;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "X");
                speedVal.x = EditorGUI.FloatField(MiniRectField(rightSideRect), speedVal.x);
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(MiniRectLabel(rightSideRect), "Y");
                speedVal.y = EditorGUI.FloatField(MiniRectField(rightSideRect), speedVal.y);
                speed.vector2Value = speedVal;

                rightSideRect.y += EditorGUIUtility.singleLineHeight + 2;

                EditorGUI.LabelField(rightSideRect, "Scale");
                rightSideRect.y += EditorGUIUtility.singleLineHeight;
                scale.floatValue = EditorGUI.FloatField(rightSideRect, scale.floatValue);

            }


            EditorGUI.indentLevel = oldIndent;
        }

        private Rect MiniRectLabel(Rect orig) {
            return new Rect(orig.x, orig.y, 10f, orig.height);
        }
        private Rect MiniRectField(Rect orig) {
            return new Rect(orig.x + 14f, orig.y, orig.width - 14f, orig.height);
        }

        /// <summary>
        /// Draws the visual box that displays a preview of the loot formation.
        /// </summary>
        /// <param name="rect">The rect to draw the box in.</param>
        /// <param name="property">The LootFormation property.</param>
        private void DrawVisualBox(Rect rect, SerializedProperty property) {

            SerializedProperty guid = property.FindPropertyRelative("guid");
            if (string.IsNullOrEmpty(guid.stringValue)) {
                guid.stringValue = System.Guid.NewGuid().ToString();
                guid.serializedObject.ApplyModifiedProperties();
            }


            GUI.SetNextControlName($"GygesLootFormationVisBox{guid.stringValue}");
            EditorGUI.LabelField(rect, GUIContent.none);

            SerializedProperty selectedLoot = property.FindPropertyRelative("selectedLoot");
            SerializedProperty locs = property.FindPropertyRelative("_locs");

            if (_backgroundColor == Color.black)
                _backgroundColor = Color.Lerp(EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255),
                    Color.black, 0.1f);

            Event current = Event.current;
            bool clickedThisFrame = current.type == EventType.MouseDown && rect.Contains(current.mousePosition);
            bool foundAClick = false;
            bool foundANewClick = false;

            if (clickedThisFrame) {

                GUI.FocusControl($"GygesLootFormationVisBox{guid.stringValue}");

                //Right click
                if (current.button == 1) {
                    GenericMenu menu = new GenericMenu();

                    Vector2 relativeMousePos = (((current.mousePosition - rect.center) * new Vector2(1f, -1f)) - new Vector2(0f, 30f)) / 50f;
                    int loot = -1;

                    // Determine which (if any) loot item we are clicking on.
                    for (int i = 0; i < locs.arraySize; i++) {
                        SerializedProperty circleLoc = locs.GetArrayElementAtIndex(i);
                        float circleScale = circleLoc.FindPropertyRelative("scale").floatValue;
                        Vector2 circlePos = rect.center + new Vector2(circleLoc.FindPropertyRelative("x").floatValue, -circleLoc.FindPropertyRelative("y").floatValue) * 50f;
                        Rect circleRect = new Rect(circlePos.x - (5f * circleScale), circlePos.y - (5f * circleScale),
                            circleScale * 10f, circleScale * 10f);

                        if (circleRect.Contains(current.mousePosition)) {
                            loot = i;
                            break;
                        }
                    }
                    

                    ContextMenuEvent ev = new ContextMenuEvent() {
                        property = property,
                        relativeMousePosition = relativeMousePos,
                        loot = loot
                    };

                    if (locs.arraySize < _maximumLootCount) {
                        menu.AddItem(new GUIContent("Add Loot"), false, CreateLootAtPoint, ev);
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Add Loot"));
                    }

                    if (loot != -1) {
                        menu.AddItem(new GUIContent("Remove Loot"), false, RemoveLootAtPoint, ev);
                        if (locs.arraySize < _maximumLootCount) {
                            menu.AddItem(new GUIContent("Duplicate Loot"), false, DuplicateLootAtPoint, ev);
                        }
                        else {
                            menu.AddDisabledItem(new GUIContent("Duplicate Loot"));
                        }
                        menu.AddItem(new GUIContent("Reset Loot"), false, ResetLootAtPoint, ev);
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Remove Loot"));
                        menu.AddDisabledItem(new GUIContent("Duplicate Loot"));
                        menu.AddDisabledItem(new GUIContent("Reset Loot"));
                    }
                    

                    menu.ShowAsContext();

                    clickedThisFrame = false;
                }
                //Middle click
                else if (current.button == 2) {
                    clickedThisFrame = false;
                }
            }


            Vector2 middle = rect.center;

            EditorGUI.DrawRect(rect, Color.black);
            EditorGUI.DrawRect(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), _backgroundColor);

            // Draw the grid behind where the visual elements will be drawn.
            for (int i = 1; i < 10; i++) {
                float x = rect.x + (i * (rect.width / 10f));
                float y = rect.y + (i * (rect.height / 10f));

                EditorGUI.DrawRect(new Rect(x, rect.y + 1, 1, rect.height - 2), Color.Lerp(_backgroundColor, Color.black, 0.5f));
                EditorGUI.DrawRect(new Rect(rect.x + 1, y, rect.width - 2, 1), Color.Lerp(_backgroundColor, Color.black, 0.5f));
            }

            if (current.type == EventType.Repaint)
                DrawCircle(new Rect(middle.x - 25, middle.y - 25, 50, 50), Color.cyan);

            //Loop through each formation circle.
            for (int i = 0; i < locs.arraySize; i++) {
                SerializedProperty loc = locs.GetArrayElementAtIndex(i);
                float locScale = loc.FindPropertyRelative("scale").floatValue;

                // Determine the position of the circle.
                Vector2 position = middle + new Vector2(loc.FindPropertyRelative("x").floatValue, -loc.FindPropertyRelative("y").floatValue) * 50f;

                // Scale the circle visually by the loot scale amount.
                Rect circlePos = new Rect(position.x - (5f * locScale), position.y - (5f * locScale), 10f * locScale, 10f * locScale);

                // If the box has been clicked this frame, check whether the circle contains the click position.
                // If this is the first previously unselected circle that contains the click position, update the selected index.
                if (clickedThisFrame && !foundANewClick) {
                    if (circlePos.Contains(current.mousePosition)) {
                        foundAClick = true;
                        if (selectedLoot.intValue != i) {
                            selectedLoot.intValue = i;
                            foundANewClick = true;
                        }
                    }
                    
                }

                if (current.type == EventType.Repaint) {

                    // Draw an arrow based on the loot's starting speed.
                    Vector2 speedTransformed = loc.FindPropertyRelative("speed").vector2Value * (Vector2.right + Vector2.down);
                    if (speedTransformed != Vector2.zero) {
                        speedTransformed = speedTransformed.normalized * Mathf.Lerp(speedTransformed.magnitude, 15f, 0.7f);
                        Vector3[] arrowValues = new Vector3[6];
                        arrowValues[0] = circlePos.center;
                        arrowValues[1] = circlePos.center + speedTransformed * 2f;
                        arrowValues[2] = arrowValues[1];
                        arrowValues[3] = (Vector3)circlePos.center + ((Vector3)speedTransformed * 1.5f) + (Quaternion.Euler(0f, 0f, 90f) * speedTransformed * 0.25f);
                        arrowValues[4] = arrowValues[1];
                        arrowValues[5] = (Vector3)circlePos.center + ((Vector3)speedTransformed * 1.5f) + (Quaternion.Euler(0f, 0f, 90f) * speedTransformed * -0.25f);
                        Handles.DrawLines(arrowValues);
                    }

                    // Draw the circle that depicts the loot's starting position.
                    DrawCircle(circlePos, selectedLoot.intValue == i ? Color.red : Color.yellow);
                }
            }

            if (clickedThisFrame && !foundANewClick && !foundAClick) {
                selectedLoot.intValue = -1;
            }

        }

        private void DrawCircle(Rect rect, Color colour) {

            //If we're not in a repaint event, don't bother with the visuals.
            if (Event.current.type != EventType.Repaint)
                return;

            if (_circleTex == null)
                _circleTex = Resources.Load<Texture2D>("Textures/UI/circleTex");

            _circleStyle = new GUIStyle();
            _circleStyle.normal.background = _circleTex;
            Color oldCol = GUI.color;
            GUI.color = colour;
            _circleStyle.Draw(rect, GUIContent.none, false, false, false, false);
            GUI.color = oldCol;
        }

        private void CreateLootAtPoint(object selection) {

            

            ContextMenuEvent ev = (ContextMenuEvent)selection;
            SerializedProperty guid = ev.property.FindPropertyRelative("guid");
            GUI.FocusControl($"GygesLootFormationVisBox{guid.stringValue}");
            SerializedProperty locs = ev.property.FindPropertyRelative("_locs");
            locs.arraySize++;
            SerializedProperty loc = locs.GetArrayElementAtIndex(locs.arraySize - 1);

            loc.FindPropertyRelative("x").floatValue = ev.relativeMousePosition.x;
            loc.FindPropertyRelative("y").floatValue = ev.relativeMousePosition.y;
            loc.FindPropertyRelative("speed").vector2Value = LootLocation.DefaultSpeed;
            loc.FindPropertyRelative("scale").floatValue = 1f;
            loc.FindPropertyRelative("prefabToUse").intValue = 0;
            ev.property.FindPropertyRelative("selectedLoot").intValue = locs.arraySize - 1;

            ev.property.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveLootAtPoint(object selection) {

            ContextMenuEvent ev = (ContextMenuEvent)selection;
            SerializedProperty guid = ev.property.FindPropertyRelative("guid");
            GUI.FocusControl($"GygesLootFormationVisBox{guid.stringValue}");
            SerializedProperty locs = ev.property.FindPropertyRelative("_locs");
            locs.DeleteArrayElementAtIndex(ev.loot);

            SerializedProperty selected = ev.property.FindPropertyRelative("selectedLoot");
            if (selected.intValue >= locs.arraySize) {
                selected.intValue = locs.arraySize - 1;
            }

            ev.property.serializedObject.ApplyModifiedProperties();
        }

        private void DuplicateLootAtPoint(object selection) {
            ContextMenuEvent ev = (ContextMenuEvent)selection;
            SerializedProperty guid = ev.property.FindPropertyRelative("guid");
            GUI.FocusControl($"GygesLootFormationVisBox{guid.stringValue}");
            SerializedProperty locs = ev.property.FindPropertyRelative("_locs");
            SerializedProperty oldLoc = locs.GetArrayElementAtIndex(ev.loot);
            locs.arraySize++;
            SerializedProperty newLoc = locs.GetArrayElementAtIndex(locs.arraySize - 1);

            newLoc.FindPropertyRelative("x").floatValue = oldLoc.FindPropertyRelative("x").floatValue;
            newLoc.FindPropertyRelative("y").floatValue = oldLoc.FindPropertyRelative("y").floatValue;
            newLoc.FindPropertyRelative("speed").vector2Value = oldLoc.FindPropertyRelative("speed").vector2Value;
            newLoc.FindPropertyRelative("scale").floatValue = oldLoc.FindPropertyRelative("scale").floatValue;
            newLoc.FindPropertyRelative("prefabToUse").intValue = oldLoc.FindPropertyRelative("prefabToUse").intValue;
            ev.property.FindPropertyRelative("selectedLoot").intValue = locs.arraySize - 1;

            ev.property.serializedObject.ApplyModifiedProperties();
        }

        private void ResetLootAtPoint(object selection) {
            ContextMenuEvent ev = (ContextMenuEvent)selection;
            SerializedProperty loc = ev.property.FindPropertyRelative("_locs").GetArrayElementAtIndex(ev.loot);

            loc.FindPropertyRelative("x").floatValue = 0f;
            loc.FindPropertyRelative("y").floatValue = 0f;
            loc.FindPropertyRelative("speed").vector2Value = LootLocation.DefaultSpeed;
            loc.FindPropertyRelative("scale").floatValue = 1f;
            loc.FindPropertyRelative("prefabToUse").intValue = 0;

            ev.property.serializedObject.ApplyModifiedProperties();
        }

        public struct ContextMenuEvent {
            public SerializedProperty property;
            public Vector2 relativeMousePosition;
            public int loot;
        }

    }

}