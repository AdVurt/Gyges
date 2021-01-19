using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gyges.Game {
    public class WaveManager : MonoBehaviour {

        public enum EndTriggerTypes {
            [InspectorName("No End")] None,
            [InspectorName("Fixed Time")] TimeBased,
            [InspectorName("All Dead")] AllEnemiesDead
        }

        public static Vector3 CubeSize { get; private set; } = new Vector3(16f * (16f / 9f), 16f, 1f);

#if UNITY_EDITOR
        [SerializeField] private Color _cubeColour = new Color(1f, 1f, 1f, 0.25f);
        private ErrorStatus[] _errorStatuses = new ErrorStatus[0];
#endif
        public GameObject[] objects = new GameObject[0];
        private HashSet<IWaveObject> _spawnedObjects = new HashSet<IWaveObject>(); //Keeps track of spawned objects.
        [SerializeField] private bool _startActive = false;
        public bool StartActive {
            get {
                return _startActive;
            }
            set {
                _startActive = value;
            }
        }
        [SerializeField] private WaveManager _nextManager = default;
        public int waveNumber = 1;
        [HideInInspector] public bool active = false;
        [SerializeField] private EndTriggerTypes _endTriggerType = EndTriggerTypes.None;
        [SerializeField] private float _timeToWait = 5f;
        private float _timer = 0f;

        public float Timer {
            get { return _timer; }
        }


        public GameState gameState;
        private bool _waveHasEnded = false;

        // Pre-activate any objects that have the "Start Logic Before Gameplay" flag set to true if this wave is set to start active.
        // This will happen, for instance, for clouds that should be visible and moving when the player spawns in.
        void Awake() {

            if (_startActive) {
                foreach (GameObject obj in objects) {

                    foreach (IWaveObject waveObject in obj.GetComponents<IWaveObject>()) {
                        if (waveObject.StartLogicBeforeGameplay) {
                            if (!obj.activeInHierarchy)
                                obj.SetActive(true);
                            if (obj.transform.parent != null)
                                obj.transform.SetParent(null, true);
                            waveObject.onDestroy += Enemy_OnDestroy;
                            _spawnedObjects.Add(waveObject);
                        }
                    }
                }
            }
        }

        void Update() {
            
            if (!Global.enableGameLogic || Global.Paused || (!active && !_startActive))
                return;

            if (_startActive && !active)
                BeginWave();

            _timer += Time.deltaTime;

            if (_endTriggerType == EndTriggerTypes.TimeBased) {
                if (_timer >= _timeToWait) {
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
                foreach (IWaveObject waveObject in obj.GetComponentsInChildren<IWaveObject>()) {
                    if (waveObject.StartLogicBeforeGameplay)
                        continue;

                    waveObject.onDestroy += Enemy_OnDestroy;
                    _spawnedObjects.Add(waveObject);
                }
            }
            objects = new GameObject[0];
            active = true;
        }

        private void Enemy_OnDestroy(IWaveObjectDestroyEventParams e) {
            if (_waveHasEnded)
                return;

            _spawnedObjects.Remove(e.waveObject);
            if (e.killedByPlayer && e.bounty > 0) {
                gameState.PendingPoints += e.bounty;
            }
            if (_spawnedObjects.Count == 0 && _endTriggerType == EndTriggerTypes.AllEnemiesDead) {
                EndWave();
            }
        }

        /// <summary>
        /// Ends this wave, if it hasn't already ended.
        /// </summary>
        public void EndWave() {
            if (_waveHasEnded)
                return;

            Destroy(gameObject);
            if (_nextManager != null) {
                _nextManager.active = true;
                _nextManager.BeginWave();
            }
            _waveHasEnded = true;
        }

        public int ObjectCount {
            get {
                return objects.Length;
            }
        }

        /// <summary>
        /// Resynchronises the object array.
        /// </summary>
        public void ResynchroniseArray() {
            objects = new GameObject[transform.childCount];
            for (int i = 0; i < objects.Length; i++) {
                objects[i] = transform.GetChild(i).gameObject;
            }
            UpdateErrors();
        }


#if UNITY_EDITOR

        [InitializeOnEnterPlayMode]
        static void OnEnterPlaymodeInEditor(EnterPlayModeOptions options) {
            foreach(WaveManager mgr in FindObjectsOfType<WaveManager>()) {
                mgr.EditorPlayVerify();
            }
        }

        void EditorPlayVerify() {

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

        void OnDrawGizmos() {

            System.Text.StringBuilder gizText = new System.Text.StringBuilder(gameObject.name);
            gizText.AppendLine();


            int status = 0;
            foreach(ErrorStatus error in _errorStatuses) {
                if (error.status > status)
                    status = error.status;
                gizText.AppendLine($"<color={ErrorStatus.GetStatusColourName(error.status)}>{error.errorMessage}</color>");
            }

            if (_errorStatuses.Length > 0) {
                Gizmos.color = ErrorStatus.GetStatusColour(status);
            }

            Gizmos.DrawWireCube(transform.position, CubeSize);
            DrawString(gizText.ToString(), transform.position);
        }

        void OnValidate() {
            UpdateErrors();
        }

        static void DrawString(string text, Vector3 worldPos, Color? colour = null) {
            Handles.BeginGUI();

            Color restoreColor = GUI.color;

            if (colour.HasValue)
                GUI.color = colour.Value;
            SceneView view = SceneView.currentDrawingSceneView;
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

        [MenuItem("GameObject/Create Wave Manager", false, 0)]
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

        #region Waves Window Highlight Logic

        public void UpdateErrors() {

            if (EditorApplication.isPlaying) {
                if (_errorStatuses.Length > 0)
                    _errorStatuses = new ErrorStatus[0];
                return;
            }

            List<ErrorStatus> errors = new List<ErrorStatus>();

            if (transform.childCount == 0) {
                errors.Add(new ErrorStatus(true, 1, "No children."));
            }
            if (transform.childCount != objects.Length) {
                errors.Add(new ErrorStatus(true, 2, "Objects array does not match hierarchy children."));
            }
            int nonWaveObjectChildren = 0;
            foreach (GameObject obj in objects) {
                if (obj == null || !obj.TryGetComponent(out IWaveObject en)) {
                    nonWaveObjectChildren++;
                }
            }
            if (nonWaveObjectChildren > 0) {
                errors.Add(new ErrorStatus(true, 2, $"{nonWaveObjectChildren} non-wave object child{(nonWaveObjectChildren > 1 ? "ren" : "")}."));
            }

            _errorStatuses = errors.ToArray();
        }

        public ErrorStatus[] GetErrors() {
            return _errorStatuses;
        }

        public struct ErrorStatus {
            public bool draw;
            public int status;
            public string errorMessage;

            public ErrorStatus(bool draw, int status, string errorMessage) {
                this.draw = draw;
                this.status = status;
                this.errorMessage = errorMessage;
            }

            public static Color GetStatusColour(int status) {
                Color result = Color.black;
                ColorUtility.TryParseHtmlString(GetStatusColourName(status), out result);
                return result;
            }

            public static string GetStatusColourName(int status) {
                switch (status) {
                    case 0:
                        return "black";
                    case 1:
                        return "yellow";
                    case 2:
                        return "red";
                    default:
                        throw new System.ArgumentException($"Unknown status ID {status}");

                }
            }
        }

        #endregion

#endif
    }
}