using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(Loadout))]
    public class LoadoutDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return property.isExpanded ? ((EditorGUIUtility.singleLineHeight + 2) * 11) - 2 : (EditorGUIUtility.singleLineHeight + 2);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            label = EditorGUI.BeginProperty(position, label, property);

            Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(pos, property.isExpanded, label);

            if (property.isExpanded) {
                int totalValue;
                EditorGUI.BeginChangeCheck();

                SerializedProperty hull = property.FindPropertyRelative("hull");
                SerializedProperty frontWeapon = property.FindPropertyRelative("frontWeapon");
                SerializedProperty rearWeapon = property.FindPropertyRelative("rearWeapon");
                SerializedProperty shield = property.FindPropertyRelative("shield");
                SerializedProperty generator = property.FindPropertyRelative("generator");
                SerializedProperty specialLeft = property.FindPropertyRelative("specialLeft");
                SerializedProperty specialRight = property.FindPropertyRelative("specialRight");
                SerializedProperty frontWeaponLevel = property.FindPropertyRelative("frontWeaponLevel");
                SerializedProperty rearWeaponLevel = property.FindPropertyRelative("rearWeaponLevel");

                if (frontWeaponLevel.intValue < 1) {
                    frontWeaponLevel.intValue = 1;
                }
                else if (frontWeaponLevel.intValue > LevelableWeapon.maximumLevel) {
                    frontWeaponLevel.intValue = LevelableWeapon.maximumLevel;
                }

                if (rearWeaponLevel.intValue < 1) {
                    rearWeaponLevel.intValue = 1;
                }
                else if (rearWeaponLevel.intValue > LevelableWeapon.maximumLevel) {
                    rearWeaponLevel.intValue = LevelableWeapon.maximumLevel;
                }


                EditorGUI.indentLevel++;

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, hull);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, frontWeapon);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, rearWeapon);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, shield);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, generator);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, specialLeft);

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, specialRight);

                GUI.enabled = frontWeapon.objectReferenceValue != null;
                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, frontWeaponLevel, new GUIContent("Front Level"));

                GUI.enabled = rearWeapon.objectReferenceValue != null;
                pos.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(pos, rearWeaponLevel, new GUIContent("Rear Level"));
                GUI.enabled = true;

                if (EditorGUI.EndChangeCheck()) {
                    property.serializedObject.ApplyModifiedProperties();
                }

                pos.y += EditorGUIUtility.singleLineHeight + 2;
                totalValue = Loadout.GetTotalValueOfParts(frontWeaponLevel.intValue, rearWeaponLevel.intValue,
                    (Hull)hull.objectReferenceValue,
                    (FrontWeapon)frontWeapon.objectReferenceValue,
                    (RearWeapon)rearWeapon.objectReferenceValue,
                    (Shield)shield.objectReferenceValue,
                    (Generator)generator.objectReferenceValue,
                    (SpecialWeapon)specialLeft.objectReferenceValue,
                    (SpecialWeapon)specialRight.objectReferenceValue
                    );
                EditorGUI.LabelField(pos, "Total Value", totalValue.ToString());
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }

}