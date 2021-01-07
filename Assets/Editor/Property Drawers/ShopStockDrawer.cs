using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(ShopStock))]
    public class ShopStockDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!property.FindPropertyRelative("foldout").boolValue)
                return EditorGUIUtility.singleLineHeight;

            int items = 8; //By default, show eight - the label plus seven item categories.
            items += property.FindPropertyRelative("hulls").arraySize;
            items += property.FindPropertyRelative("frontWeapons").arraySize;
            items += property.FindPropertyRelative("rearWeapons").arraySize;
            items += property.FindPropertyRelative("shields").arraySize;
            items += property.FindPropertyRelative("generators").arraySize;
            items += property.FindPropertyRelative("leftSpecials").arraySize;
            items += property.FindPropertyRelative("rightSpecials").arraySize;

            return ((EditorGUIUtility.singleLineHeight+2) * items)-2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float height = EditorGUIUtility.singleLineHeight;

            SerializedProperty foldoutProperty = property.FindPropertyRelative("foldout");
            bool foldout = EditorGUI.Foldout(pos, foldoutProperty.boolValue, label);
            foldoutProperty.boolValue = foldout;

            if (foldout) {

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                SerializedProperty hulls = property.FindPropertyRelative("hulls");
                SerializedProperty frontWeapons = property.FindPropertyRelative("frontWeapons");
                SerializedProperty rearWeapons = property.FindPropertyRelative("rearWeapons");
                SerializedProperty shields = property.FindPropertyRelative("shields");
                SerializedProperty generators = property.FindPropertyRelative("generators");
                SerializedProperty leftSpecials = property.FindPropertyRelative("leftSpecials");
                SerializedProperty rightSpecials = property.FindPropertyRelative("rightSpecials");

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Hulls");
                DrawItems(hulls, ref pos);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Front Weapons");
                DrawItems(frontWeapons, ref pos);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Rear Weapons");
                DrawItems(rearWeapons, ref pos, true);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Shields");
                DrawItems(shields, ref pos);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Generators");
                DrawItems(generators, ref pos);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Left Specials");
                DrawItems(leftSpecials, ref pos, true);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(pos, "Right Specials");
                DrawItems(rightSpecials, ref pos, true);

                EditorGUI.indentLevel = oldIndent;
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private void DrawItems(SerializedProperty itemArray, ref Rect position, bool optionalItem = false) {

            GUI.enabled = itemArray.arraySize > 0;
            if (GUI.Button(new Rect(position.x + position.width - 50, position.y, 25, position.height), "-")) {
                itemArray.arraySize--;
            }

            GUI.enabled = itemArray.arraySize < ShopStock.maximumItemListSize;
            //Enforce maximum item list size;
            if (itemArray.arraySize > ShopStock.maximumItemListSize)
                itemArray.arraySize = ShopStock.maximumItemListSize;
            if (GUI.Button(new Rect(position.x + position.width - 25, position.y, 25, position.height), "+")) {
                itemArray.arraySize++;
            }
            GUI.enabled = true;

            EditorGUI.indentLevel++;
            for (int i = 0; i < itemArray.arraySize; i++) {
                position.y += EditorGUIUtility.singleLineHeight + 2;
                SerializedProperty prop = itemArray.GetArrayElementAtIndex(i);

                EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - 64, position.height), prop, GUIContent.none);

                if (prop.objectReferenceValue != null) {
                    int oldIndent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;
                    int itemCost = ((ShipPart)prop.objectReferenceValue).GetCost(1);
                    EditorGUI.LabelField(new Rect(position.x + position.width - 48, position.y, 48, position.height), itemCost.ToString());
                    EditorGUI.indentLevel = oldIndent;
                }
            }
            EditorGUI.indentLevel--;

        }

    }

}