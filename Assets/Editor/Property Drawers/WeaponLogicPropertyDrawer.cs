using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(WeaponLogic))]
    public class WeaponLogicPropertyDrawer : PropertyDrawer {


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            ProjectileFormationPropertyDrawer.height + EditorGUIUtility.singleLineHeight * 3 + 6;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty powerCost = property.FindPropertyRelative("powerCost");
            SerializedProperty reloadTime = property.FindPropertyRelative("reloadTime");
            SerializedProperty damage = property.FindPropertyRelative("damage");
            SerializedProperty formation = property.FindPropertyRelative("formation");

            EditorGUI.PropertyField(pos, powerCost);
            pos.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(pos, reloadTime);
            pos.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(pos, damage);
            pos.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width, ProjectileFormationPropertyDrawer.height), formation);

        }

    }

}