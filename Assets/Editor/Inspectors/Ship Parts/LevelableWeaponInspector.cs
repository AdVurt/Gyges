using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyges.Game;

namespace Gyges.CustomEditors {

    public abstract class LevelableWeaponInspector : ShipPartInspector {

        private static ProjectileFormation? _formationClipboard = null;

        // Mass updates
        public static float massPowerCostUpdate = 0f;
        public static float massReloadTimeUpdate = 0.2f;
        public static float massDamageUpdate = 5f;
        public static Vector2 massSpeedUpdate = new Vector2(0f, 15f);
        public static float massRotationUpdate = 0f;
        public static float massScaleUpdate = 0f;
        public static int massPrefabUpdate = 0;
        public static float massDamageMultiplierUpdate = 1f;

        private LevelableWeapon _target;
        private SerializedProperty _weaponType;
        private SerializedProperty _projectilePrefabs;
        private SerializedProperty _levelStats;
        private SerializedProperty _fireSound;

        // Rear only
        private bool _altMode = false;
        private bool _canHaveAltStats;

        private int _level;
        private Rect _formationEnumField = Rect.zero;
        private string[] _prefabStrings;
        private int[] _prefabIndexes;
        private Color[] _prefabColours;

        new void OnEnable() {
            base.OnEnable();
            _target = (LevelableWeapon)target;
            _weaponType = serializedObject.FindProperty("weaponType");

            _canHaveAltStats = _target.ShipPartType == ShipPart.PartType.RearWeapon;
            if (_canHaveAltStats) {
                _levelStats = serializedObject.FindProperty(_altMode ? "altLevelStats" : "levelStats");
            }
            else {
                _levelStats = serializedObject.FindProperty("levelStats");
            }
            _projectilePrefabs = serializedObject.FindProperty("projectilePrefabs");
            _fireSound = serializedObject.FindProperty("fireSound");
            _level = 0;

            UpdatePrefabData();
        }

        private void UpdatePrefabData() {
            
            // Remove all blank values before generating the prefab name lists
            for (int i = _projectilePrefabs.arraySize - 1; i >= 0; i--) {
                if (_projectilePrefabs.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    _projectilePrefabs.DeleteArrayElementAtIndex(i);
            }
            _prefabStrings = new string[_projectilePrefabs.arraySize];
            _prefabIndexes = new int[_prefabStrings.Length];

            for (int i = 0; i < _projectilePrefabs.arraySize; i++) {
                GameObject obj = (GameObject)_projectilePrefabs.GetArrayElementAtIndex(i).objectReferenceValue;
                _prefabStrings[i] = obj.name;
                _prefabIndexes[i] = i;
            }
        }

        public override void OnInspectorGUI() {
            DrawShipPartInspector();

            EditorGUILayout.LabelField("Levelable Weapon-Specific Data", EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUILayout.PropertyField(_weaponType);

            bool madeChangesToArray = false;
            if (_projectilePrefabs.arraySize == 0) {
                _projectilePrefabs.arraySize = 1;
                madeChangesToArray = true;
            }
            for (int i = 0; i < _projectilePrefabs.arraySize; i++) {
                using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.PrefixLabel(i == 0 ? "Projectile Prefabs" : " ");
                    EditorGUI.BeginChangeCheck();
                    GameObject prefab = (GameObject)EditorGUILayout.ObjectField(_projectilePrefabs.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject), false);
                    if (EditorGUI.EndChangeCheck()) {
                        _projectilePrefabs.GetArrayElementAtIndex(i).objectReferenceValue = prefab;
                        madeChangesToArray = true;
                    }
                }
            }
            if (_projectilePrefabs.GetArrayElementAtIndex(_projectilePrefabs.arraySize - 1).objectReferenceValue != null) {
                using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {

                    Color oldGUICol = GUI.color;
                    GUI.color = new Color(oldGUICol.r, oldGUICol.g, oldGUICol.b, 0.75f);

                    EditorGUILayout.PrefixLabel(" ");
                    GameObject extraPrefab = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), false);
                    if (extraPrefab != null) {
                        _projectilePrefabs.arraySize++;
                        _projectilePrefabs.GetArrayElementAtIndex(_projectilePrefabs.arraySize - 1).objectReferenceValue = extraPrefab;
                        madeChangesToArray = true;
                    }

                    GUI.color = oldGUICol;

                }
            }

            if (madeChangesToArray) {
                EditorUtility.SetDirty(_target);
                UpdatePrefabData();
            }

            if (_projectilePrefabs.arraySize == 0 || (_projectilePrefabs.arraySize == 1 && _projectilePrefabs.GetArrayElementAtIndex(0).objectReferenceValue == null)) {
                EditorGUILayout.HelpBox("Please select at least one prefab.", MessageType.Error);
            }

            EditorGUILayout.Space();


            if (_levelStats.arraySize != LevelableWeapon.maximumLevel)
                _levelStats.arraySize = LevelableWeapon.maximumLevel;

            if (_canHaveAltStats) {
                using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    _altMode = GUILayout.Toggle(_altMode, "Alternate Mode");
                    if (EditorGUI.EndChangeCheck()) {
                        _levelStats = serializedObject.FindProperty(_altMode ? "altLevelStats" : "levelStats");
                    }
                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(2);
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {

                GUIStyle paneStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) {
                    imagePosition = ImagePosition.ImageOnly
                };

                GUILayout.FlexibleSpace();
                using (EditorGUILayout.HorizontalScope sc2 = new EditorGUILayout.HorizontalScope()) {

                    GUILayout.Space(paneStyle.fixedWidth);
                    EditorGUI.BeginDisabledGroup(_level == 0);
                    if (GUILayout.Button("<", GUILayout.Width(40f))) {
                        _level--;
                        _levelStats.GetArrayElementAtIndex(_level).FindPropertyRelative("formation").FindPropertyRelative("selectedProjectile").intValue = -1;
                        GUI.FocusControl("");
                    }
                    EditorGUI.EndDisabledGroup();

                    GUILayout.Space(5);
                    GUILayout.Label($"Level {(_level + 1).ToString()}", EditorStyles.boldLabel);
                    GUILayout.Space(5);

                    EditorGUI.BeginDisabledGroup(_level == LevelableWeapon.maximumLevel - 1);
                    if (GUILayout.Button(">", GUILayout.Width(40f))) {
                        _level++;
                        _levelStats.GetArrayElementAtIndex(_level).FindPropertyRelative("formation").FindPropertyRelative("selectedProjectile").intValue = -1;
                        GUI.FocusControl("");
                    }
                    EditorGUI.EndDisabledGroup();

                }
                GUILayout.FlexibleSpace();

                Rect ctRect = EditorGUILayout.GetControlRect(GUILayout.Width(paneStyle.fixedWidth));
                if (GUI.Button(ctRect, GUIContent.none, paneStyle)) {
                    GenericMenu menu = new GenericMenu();
                    
                    menu.AddItem(new GUIContent("Copy"), false, CopyFormation);

                    if (_formationClipboard.HasValue) {
                        menu.AddItem(new GUIContent("Paste"), false, PasteFormation);
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }

                    menu.AddSeparator("");

                    menu.AddItem(new GUIContent("Copy Formation to all Levels"), false, CopyFormationToAllLevels);

                    menu.DropDown(ctRect);
                }

            }
            // This calls the ProjectileFormationPropertyDrawer drawer.
            SerializedProperty currentLevelFormation = _levelStats.GetArrayElementAtIndex(_level);
            EditorGUILayout.PropertyField(currentLevelFormation);

            // Draw the prefab selector field above the drawer grid if we have at least two prefabs and a projectile is selected.
            if (_projectilePrefabs.arraySize > 1) {
                if (Event.current.type == EventType.Repaint) {

                    _formationEnumField = GUILayoutUtility.GetLastRect();
                    float xMiddle = _formationEnumField.center.x;

                    _formationEnumField.x = xMiddle - 90f;
                    _formationEnumField.y += EditorGUIUtility.singleLineHeight * 4 + 8;
                    _formationEnumField.width = 180f;
                    _formationEnumField.height = EditorGUIUtility.singleLineHeight;
                }
                int selectedProjectile = currentLevelFormation.FindPropertyRelative("formation").FindPropertyRelative("selectedProjectile").intValue;
                    
                if (selectedProjectile > -1) {
                    SerializedProperty selectedPrefab = currentLevelFormation.FindPropertyRelative("formation").FindPropertyRelative("_locs").GetArrayElementAtIndex(selectedProjectile).FindPropertyRelative("prefabToUse");
                    selectedPrefab.intValue = EditorGUI.IntPopup(_formationEnumField, selectedPrefab.intValue, _prefabStrings, _prefabIndexes);
                }
                
            }

            // Show the cost.
            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Cost: {_target.GetCost(_level + 1).ToString()}");
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_fireSound);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(17f);
            EditorGUILayout.LabelField("Mass updates", EditorStyles.boldLabel);

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massPowerCostUpdate = EditorGUILayout.FloatField("Power cost", massPowerCostUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        log.powerCost = massPowerCostUpdate;
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massReloadTimeUpdate = EditorGUILayout.FloatField("Reload time", massReloadTimeUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        log.reloadTime = massReloadTimeUpdate;
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massDamageUpdate = EditorGUILayout.FloatField("Damage", massDamageUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        log.damage = massDamageUpdate;
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massSpeedUpdate = EditorGUILayout.Vector2Field("Projectile speed", massSpeedUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                        for (int i = 0; i < log.formation.Size; i++) {
                            locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, massSpeedUpdate, log.formation[i].rotation);
                        }
                        log.formation = new ProjectileFormation(locs);
                    }
                    EditorUtility.SetDirty(_target);
                }

            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massRotationUpdate = EditorGUILayout.FloatField("Rotation", massRotationUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                        for (int i = 0; i < log.formation.Size; i++) {
                            locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, log.formation[i].speed, massRotationUpdate);
                        }
                        log.formation = new ProjectileFormation(locs);
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massScaleUpdate = EditorGUILayout.FloatField("Scale", massScaleUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                        for (int i = 0; i < log.formation.Size; i++) {
                            locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, log.formation[i].speed, log.formation[i].rotation, massScaleUpdate);
                        }
                        log.formation = new ProjectileFormation(locs);
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massPrefabUpdate = EditorGUILayout.IntField("Prefab ID", massPrefabUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                        for (int i = 0; i < log.formation.Size; i++) {
                            locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, log.formation[i].speed, log.formation[i].rotation, log.formation[i].scale, massPrefabUpdate);
                        }
                        log.formation = new ProjectileFormation(locs);
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {
                massDamageMultiplierUpdate = EditorGUILayout.FloatField("Damage Multiplier", massDamageMultiplierUpdate);
                if (GUILayout.Button("Go", GUILayout.MaxWidth(40))) {
                    foreach (WeaponLogic log in _target.levelStats) {
                        ProjectileLocation[] locs = new ProjectileLocation[log.formation.Size];
                        for (int i = 0; i < log.formation.Size; i++) {
                            locs[i] = new ProjectileLocation(log.formation[i].x, log.formation[i].y, log.formation[i].speed, log.formation[i].rotation, log.formation[i].scale, log.formation[i].prefabToUse, massDamageMultiplierUpdate);
                        }
                        log.formation = new ProjectileFormation(locs);
                    }
                    EditorUtility.SetDirty(_target);
                }
            }

        }



        public override bool HasPreviewGUI() => true;

        public override void OnPreviewGUI(Rect r, GUIStyle background) {

            Rect rect = new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight);
            int totalCost = 0;
            for (int i = 0; i < _target.levelStats.Length; i++) {
                int cost = _target.LevelCost(i + 1);
                totalCost += cost;
                EditorGUI.LabelField(rect, $"Level {(i + 1).ToString()} cost: ", $"{cost} ({totalCost} total)");
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }


        }

        private void CopyFormation() {
            _formationClipboard = _target.levelStats[_level].formation;
        }

        private void PasteFormation() {
            if (!_formationClipboard.HasValue)
                return;

            SerializedProperty selectedFormation = _levelStats.GetArrayElementAtIndex(_level).FindPropertyRelative("formation").FindPropertyRelative("_locs");
            selectedFormation.arraySize = _formationClipboard.Value.Size;
            
            for (int i = 0; i < selectedFormation.arraySize; i++) {
                SerializedProperty loc = selectedFormation.GetArrayElementAtIndex(i);
                loc.FindPropertyRelative("x").floatValue = _formationClipboard.Value[i].x;
                loc.FindPropertyRelative("y").floatValue = _formationClipboard.Value[i].y;
                loc.FindPropertyRelative("speed").vector2Value = _formationClipboard.Value[i].speed;
                loc.FindPropertyRelative("rotation").floatValue = _formationClipboard.Value[i].rotation;
                loc.FindPropertyRelative("scale").floatValue = _formationClipboard.Value[i].scale;
                loc.FindPropertyRelative("damageMultiplier").floatValue = _formationClipboard.Value[i].damageMultiplier;
                loc.FindPropertyRelative("prefabToUse").intValue = _formationClipboard.Value[i].prefabToUse;
            }

            serializedObject.ApplyModifiedProperties();

        }

        private void CopyFormationToAllLevels() {

            ProjectileFormation f = _target.levelStats[_level].formation;
            

            for (int i = 0; i < _levelStats.arraySize; i++) {
                SerializedProperty formation = _levelStats.GetArrayElementAtIndex(i).FindPropertyRelative("formation").FindPropertyRelative("_locs");
                formation.arraySize = f.Size;

                for (int j = 0; j < formation.arraySize; j++) {
                    SerializedProperty loc = formation.GetArrayElementAtIndex(j);
                    loc.FindPropertyRelative("x").floatValue = f[j].x;
                    loc.FindPropertyRelative("y").floatValue = f[j].y;
                    loc.FindPropertyRelative("speed").vector2Value = f[j].speed;
                    loc.FindPropertyRelative("rotation").floatValue = f[j].rotation;
                    loc.FindPropertyRelative("scale").floatValue = f[j].scale;
                    loc.FindPropertyRelative("damageMultiplier").floatValue = f[j].damageMultiplier;
                    loc.FindPropertyRelative("prefabToUse").intValue = f[j].prefabToUse;

                }

            }

            serializedObject.ApplyModifiedProperties();
        }

    }

    [CustomEditor(typeof(FrontWeapon))]
    public class FrontWeaponInspector : LevelableWeaponInspector {  }

    [CustomEditor(typeof(RearWeapon))]
    public class RearWeaponInspector : LevelableWeaponInspector { }
}
