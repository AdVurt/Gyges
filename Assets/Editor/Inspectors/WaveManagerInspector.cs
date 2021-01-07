using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Gyges.Game;

namespace Gyges.CustomEditors {

    [CustomEditor(typeof(WaveManager)), CanEditMultipleObjects]
    public class WaveManagerInspector : Editor {

        WaveManager[] _targets;
        SerializedProperty _cubeColour;
        SerializedProperty _objects;
        SerializedProperty _startActive;
        SerializedProperty _nextManager;
        SerializedProperty _waveNumber;
        SerializedProperty _endTriggerType;
        SerializedProperty _timeToWait;

        void OnEnable() {

            _targets = new WaveManager[serializedObject.targetObjects.Length];
            for (int i = 0; i < serializedObject.targetObjects.Length; i++) {
                _targets[i] = (WaveManager)serializedObject.targetObjects[i];
            }
            _objects = serializedObject.FindProperty("objects");
            _cubeColour = serializedObject.FindProperty("_cubeColour");
            _startActive = serializedObject.FindProperty("_startActive");
            _nextManager = serializedObject.FindProperty("_nextManager");
            _waveNumber = serializedObject.FindProperty("waveNumber");
            _endTriggerType = serializedObject.FindProperty("_endTriggerType");
            _timeToWait = serializedObject.FindProperty("_timeToWait");
        }

        private Vector3 CalculateDefaultWavePosition(int waveNumber) {
            return new Vector3(0f, waveNumber * (WaveManager.CubeSize.y + 1), 0f);
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.LabelField("Manager Info", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_startActive);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_waveNumber);
            if (GUILayout.Button("Position According to Wave Number")) {
                foreach(WaveManager target in _targets) {
                    target.transform.position = CalculateDefaultWavePosition(_waveNumber.intValue);
                }
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_endTriggerType, new GUIContent("End When"));
            WaveManager.EndTriggerTypes trigType = (WaveManager.EndTriggerTypes)_endTriggerType.enumValueIndex;
            if (trigType == WaveManager.EndTriggerTypes.TimeBased) {
                EditorGUILayout.PropertyField(_timeToWait);
            }
            EditorGUILayout.PropertyField(_nextManager);



            EditorGUILayout.Space();

            int activeCount = 0;
            int inactiveCount = 0;
            foreach (WaveManager target in _targets) {
                foreach (GameObject obj in target.objects) {
                    if (obj == null)
                        continue;

                    if (obj.activeInHierarchy)
                        activeCount++;
                    else
                        inactiveCount++;
                }
            }

            EditorGUILayout.LabelField("Object Array", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Number of Objects", $"{activeCount + inactiveCount}");
            EditorGUILayout.LabelField("Enabled", $"{activeCount}");
            EditorGUILayout.LabelField("Disabled", $"{inactiveCount}");
            using (EditorGUILayout.HorizontalScope sc = new EditorGUILayout.HorizontalScope()) {

                EditorGUI.BeginDisabledGroup(inactiveCount == 0);
                if (GUILayout.Button("Enable all objects")) {
                    foreach (WaveManager target in _targets) {
                        for (int i = 0; i < target.objects.Length; i++) {
                            target.objects[i].SetActive(true);
                        }
                    }
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(activeCount == 0);
                if (GUILayout.Button("Disable all objects")) {
                    foreach (WaveManager target in _targets) {
                        for (int i = 0; i < target.objects.Length; i++) {
                            target.objects[i].SetActive(false);
                        }
                    }
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                EditorGUI.EndDisabledGroup();
            }

            if (GUILayout.Button(new GUIContent($"Sync Object Array{(_targets.Length > 1 ? "s" : "")}", $"Rebuilds the object array with all of {(_targets.Length > 1 ? "these objects'" : "this object's")} child objects."))) {
                foreach (WaveManager target in _targets) {
                    target.objects = new GameObject[target.transform.childCount];
                    for (int i = 0; i < target.objects.Length; i++) {
                        target.objects[i] = target.transform.GetChild(i).gameObject;
                    }
                }
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            //Do not show manual array editing in multi-edit mode. Each object should belong to only one wave.
            if (!serializedObject.isEditingMultipleObjects) {
                EditorGUILayout.PropertyField(_objects, new GUIContent("Manual Object Array Editing"));
                if (_objects.arraySize != _targets[0].transform.childCount) {
                    EditorGUILayout.HelpBox("The number of children assigned to this object is different than the Object array. Sync is recommended.", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_cubeColour, new GUIContent("Playable area colour"));

            serializedObject.ApplyModifiedProperties();
        }

    }
}