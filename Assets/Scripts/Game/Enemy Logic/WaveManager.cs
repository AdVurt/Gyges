using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.Game {
    public class WaveManager : MonoBehaviour {

        public enum EndTriggerTypes {
            [InspectorName("No End")] None,
            [InspectorName("Fixed Time")] TimeBased,
            [InspectorName("All Dead")] AllEnemiesDead
        }

        public static Vector3 CubeSize { get; private set; } = new Vector3(16f * (16f / 9f), 16f, 1f);


        [SerializeField] private Color _cubeColour = new Color(1f, 1f, 1f, 0.25f);
        public GameObject[] objects = new GameObject[0];
        private HashSet<IWaveObject> _spawnedObjects = new HashSet<IWaveObject>(); //Keeps track of spawned objects.
        [SerializeField] private bool _startActive = false;
        [SerializeField] private WaveManager _nextManager = default;
        public int waveNumber = 1;
        [HideInInspector] public bool active = false;
        [SerializeField] private EndTriggerTypes _endTriggerType = EndTriggerTypes.None;
        [SerializeField] private float _timeToWait = 5f;

#if UNITY_EDITOR
        void Awake() {

            int objectsThatStartedActive = 0;
            foreach (GameObject obj in objects) {
                if (obj.activeInHierarchy) {
                    obj.SetActive(false);
                    objectsThatStartedActive++;
                }
            }
            if (objectsThatStartedActive > 0)
            Debug.LogWarning($"<b>{name}</b> - {objectsThatStartedActive} enemy wave object{(objectsThatStartedActive == 1 ? "" : "s")} started the scene in an active state. " + 
                $"{(objectsThatStartedActive == 1 ? "This has" : "They have")} been disabled.", this);
        }
#endif

        void Update() {
            if (!Global.enableGameLogic || Global.Paused || (!active && !_startActive))
                return;

            if (_startActive && !active)
                BeginWave();

            if (_endTriggerType == EndTriggerTypes.TimeBased) {
                _timeToWait -= Time.deltaTime;
                if (_timeToWait <= 0f) {
                    EndWave();
                }
            }
        }

        /// <summary>
        /// Starts this wave, if it hasn't already started.
        /// </summary>
        public void BeginWave() {
            transform.position = Vector3.zero;
            transform.DetachChildren();
            foreach (GameObject obj in objects) {
                obj.SetActive(true);
                foreach (IWaveObject waveObject in obj.GetComponents<IWaveObject>()) {
                    waveObject.OnDestroy += Enemy_OnDestroy;
                    _spawnedObjects.Add(waveObject);
                }
            }
            objects = new GameObject[0];
            active = true;
        }

        private void Enemy_OnDestroy(IWaveObjectDestroyEventParams e) {
            _spawnedObjects.Remove(e.waveObject);
            if (_spawnedObjects.Count == 0 && _endTriggerType == EndTriggerTypes.AllEnemiesDead) {
                EndWave();
            }
        }

        /// <summary>
        /// Ends this wave, if it hasn't already ended.
        /// </summary>
        public void EndWave() {
            Destroy(gameObject);
            if (_nextManager != null) {
                _nextManager.active = true;
                _nextManager.BeginWave();
            }
        }

        public int ObjectCount {
            get {
                return objects.Length;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos() {

            System.Text.StringBuilder gizText = new System.Text.StringBuilder(gameObject.name);
            gizText.AppendLine();
            int status = 0;


            if (transform.childCount == 0) {
                gizText.AppendLine("<color=yellow>No children</color>");
                status = 1;
            }
            if (transform.childCount != objects.Length) {
                gizText.AppendLine("<color=red>Objects array does not match hierarchy children</color>");
                status = 2;
            }
            int nonWaveObjectChildren = 0;
            foreach(GameObject obj in objects) {
                if (obj == null || !obj.TryGetComponent(out IWaveObject en)) {
                    nonWaveObjectChildren++;
                }
            }
            if (nonWaveObjectChildren > 0) {
                gizText.AppendLine($"<color=red>{nonWaveObjectChildren} non-wave object child{(nonWaveObjectChildren > 1 ? "ren" : "")}</color>");
                status = 2;
            }

            switch (status) {
                case 0: Gizmos.color = _cubeColour;     break;
                case 1: Gizmos.color = Color.yellow;    break;
                case 2: Gizmos.color = Color.red;       break;
                default: throw new System.Exception("Unknown gizmo status.");
            }

            Gizmos.DrawWireCube(transform.position, CubeSize);
            DrawString(gizText.ToString(), transform.position);
        }


        static void DrawString(string text, Vector3 worldPos, Color? colour = null) {
            Handles.BeginGUI();

            var restoreColor = GUI.color;

            if (colour.HasValue)
                GUI.color = colour.Value;
            var view = SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0) {
                GUI.color = restoreColor;
                Handles.EndGUI();
                return;
            }

            GUIStyle st = new GUIStyle(EditorStyles.boldLabel) {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            Vector2 size = st.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, st);
            GUI.color = restoreColor;
            Handles.EndGUI();
        }

        [MenuItem("GameObject/Gyges/Wave Manager", false, 10)]
        public static void CreateWaveManager(MenuCommand menuCommand) {

            int highestExistingWaveNumber = 0;

            foreach (WaveManager manager in FindObjectsOfType<WaveManager>()) {
                if (manager.waveNumber > highestExistingWaveNumber)
                    highestExistingWaveNumber = manager.waveNumber;
            }

            GameObject go = new GameObject($"Wave {(highestExistingWaveNumber+1).ToString("0#")}", typeof(WaveManager));
            WaveManager mgr = go.GetComponent<WaveManager>();
            mgr.waveNumber = highestExistingWaveNumber+1;
            if (highestExistingWaveNumber == 0)
                mgr._startActive = true;
            go.transform.position = CalculateDefaultWavePosition(mgr.waveNumber);
            Undo.RegisterCreatedObjectUndo(go, "Create Wave Manager");
            Selection.activeObject = go;

        }

        public static Vector3 CalculateDefaultWavePosition(int waveNumber) {
            return new Vector3(0f, waveNumber * (CubeSize.y + 1), 0f);
        }
#endif
    }
}