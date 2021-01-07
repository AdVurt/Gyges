using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(SpecialWeapon))]
    public class SpecialInspector : ShipPartInspector {

        private SpecialWeapon _target;
        public static Vector2 massSpeedUpdate = new Vector2(0f, 10f);

        private SerializedProperty _weaponType;
        private SerializedProperty _projectilePrefab;
        private SerializedProperty _logic;
        private SerializedProperty _fireSound;
        private SerializedProperty _maxAmmo;
        private SerializedProperty _ammoRequirement;
        private SerializedProperty _rechargeRate;

        new void OnEnable() {
            base.OnEnable();
            _target = (SpecialWeapon)target;
            _weaponType = serializedObject.FindProperty("weaponType");
            _projectilePrefab = serializedObject.FindProperty("projectilePrefab");
            _logic = serializedObject.FindProperty("logic");
            _fireSound = serializedObject.FindProperty("fireSound");
            _maxAmmo = serializedObject.FindProperty("maxAmmo");
            _ammoRequirement = serializedObject.FindProperty("ammoRequirement");
            _rechargeRate = serializedObject.FindProperty("rechargeRate");
        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();
            EditorGUILayout.LabelField("Special Weapon-Specific Data", EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUILayout.PropertyField(_weaponType);
            EditorGUILayout.PropertyField(_projectilePrefab);
            EditorGUILayout.PropertyField(_logic);
            EditorGUILayout.PropertyField(_fireSound);
            EditorGUILayout.PropertyField(_maxAmmo);
            EditorGUILayout.PropertyField(_ammoRequirement);
            EditorGUILayout.PropertyField(_rechargeRate);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(17f);
            EditorGUILayout.LabelField("Mass updates", EditorStyles.boldLabel);
            massSpeedUpdate = EditorGUILayout.Vector2Field("Projectile speed", massSpeedUpdate);

            if (GUILayout.Button("Mass Speed Update")) {
                WeaponLogic log = _target.logic;
                ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                for (int i = 0; i < log.formation.Size; i++) {
                    locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, massSpeedUpdate, log.formation[i].rotation);
                }
                log.formation = new ProjectileFormation(locs);

                EditorUtility.SetDirty(_target);
            }

        }

    }

}