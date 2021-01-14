using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using Gyges.Game;
using System.Linq;
using System.Text;

namespace Gyges.CustomEditors {

    public class WavesWindow : EditorWindow {

        private static GUIStyle _plainHeader;
        private static GUIStyle _yellowHeader;
        private static GUIStyle _redHeader;
        private static GUIStyle _wrappedLabelSkin;
        private static WaveManager[] _managers = new WaveManager[0];
        private static Dictionary<WaveManager,bool> _managersExpanded = new Dictionary<WaveManager, bool>();

        [MenuItem("Window/Gyges/Waves")]
        public static void ShowWindow() {
            GetWindow<WavesWindow>().titleContent = new GUIContent("Waves");
        }

        private void UpdateCache() {
            _managers = FindObjectsOfType<WaveManager>();
            Array.Sort(_managers, (x,y) => x.waveNumber.CompareTo(y.waveNumber));

            Dictionary<WaveManager, bool> newMgrsExpandedDict = new Dictionary<WaveManager, bool>();
            for (int i = 0; i < _managers.Length; i++) {
                newMgrsExpandedDict.Add(_managers[i] , _managersExpanded.Keys.Contains(_managers[i]) ? _managersExpanded[_managers[i]] : false );
            }
            _managersExpanded = newMgrsExpandedDict;

        }

        void OnEnable() {
            UpdateCache();
        }

        void OnHierarchyChange() {
            UpdateCache();
            foreach(WaveManager mgr in _managers) {
                mgr.UpdateErrors();
            }
            Repaint();
        }


        void OnGUI() {
            
            if (_managers.Length == 0) {
                if (_wrappedLabelSkin == null)
                    _wrappedLabelSkin = new GUIStyle(GUI.skin.label) {
                        wordWrap = true,
                        alignment = TextAnchor.MiddleCenter
                    };
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("The current scene does not contain any WaveManager objects.", _wrappedLabelSkin);
                GUILayout.FlexibleSpace();
                return;
            }

            // If the code reaches here, there is at least one WaveManager.
            foreach(WaveManager mgr in _managers) {
                DrawWave(mgr);
            }


        }


        private void DrawWave(WaveManager manager) {

            int status = 0;
            StringBuilder sb = new StringBuilder();

            foreach (WaveManager.ErrorStatus error in manager.GetErrors()) {
                if (error.status > status)
                    status = error.status;
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append(error.errorMessage);
            }

            if (_plainHeader == null) {
                _plainHeader = new GUIStyle(EditorStyles.foldout) {
                    fontStyle = FontStyle.Bold
                };
            }
            if (_yellowHeader == null) {
                _yellowHeader = new GUIStyle(_plainHeader);
                _yellowHeader.normal.textColor = Color.yellow;
                _yellowHeader.onNormal.textColor = Color.yellow;
                _yellowHeader.hover.textColor = Color.yellow;
                _yellowHeader.onHover.textColor = Color.yellow;
                _yellowHeader.focused.textColor = Color.yellow;
                _yellowHeader.onFocused.textColor = Color.yellow;
                _yellowHeader.active.textColor = Color.yellow;
                _yellowHeader.onActive.textColor = Color.yellow;
            }
            if (_redHeader == null) {
                _redHeader = new GUIStyle(_plainHeader);
                _redHeader.normal.textColor = Color.red;
                _redHeader.onNormal.textColor = Color.red;
                _redHeader.hover.textColor = Color.red;
                _redHeader.onHover.textColor = Color.red;
                _redHeader.focused.textColor = Color.red;
                _redHeader.onFocused.textColor = Color.red;
                _redHeader.active.textColor = Color.red;
                _redHeader.onActive.textColor = Color.red;
            }

            GUIStyle style;
            switch (status) {
                case 0:
                    style = _plainHeader;
                    break;
                case 1:
                    style = _yellowHeader;
                    break;
                case 2:
                    style = _redHeader;
                    break;
                default:
                    throw new MissingReferenceException($"No known error status {status}.");
            }

            _managersExpanded[manager] = EditorGUILayout.Foldout(_managersExpanded[manager], manager.name, true, style);

            if (_managersExpanded[manager]) {
                //Past this point, everything is in the expanded foldout.

                if (status != 0) {
                    EditorGUILayout.HelpBox(sb.ToString(), status == 1 ? MessageType.Warning : MessageType.Error);
                    if (GUILayout.Button("Re-synchronise")) {
                        manager.ResynchroniseArray();
                        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                        SceneView.RepaintAll();
                    }
                }


                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                bool val = EditorGUILayout.Toggle("Starting Wave", manager.StartActive);
                if (EditorGUI.EndChangeCheck()) {
                    if (val) {
                        foreach (WaveManager mgr in _managers) {
                            mgr.StartActive = mgr == manager;
                        }
                        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    }
                    else {
                        manager.StartActive = false;
                        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    }
                }

                if (GUILayout.Button("View in Inspector")) {
                    Selection.activeGameObject = manager.gameObject;
                }

                EditorGUI.indentLevel--;

            }

        }

    }

}