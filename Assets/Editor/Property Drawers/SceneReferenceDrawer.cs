// Based on Ryan Hipple's SceneReferenceEditor.cs code from the following link:
// https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SceneReference/Code/Editor/SceneReferenceEditor.cs

using System;
using UnityEngine;
using UnityEditor;
using Gyges.Utility;

namespace Gyges.CustomEditors {

    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer {

        #region -- Constants --------------------------------------------------
        private const string _TOOLTIP_SCENE_MISSING = 
            "Scene is not in build settings.";
        private const string _ERROR_SCENE_MISSING = 
            "You are referencing a scene that is not added to the build. Add it to the editor build settings now?";
        private const string _TOOLTIP_SCENE_DISABLED =
            "Scene is not enabled in build settings.";
        private const string _ERROR_SCENE_DISABLED =
            "You are referencing a scene that is not active in the build. Enable it in the build settings now?";
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        private SerializedProperty _scene;
        private SerializedProperty _sceneName;
        private SerializedProperty _sceneIndex;
        private SerializedProperty _sceneEnabled;
        private SceneAsset _sceneAsset;
        private string _sceneAssetPath;
        private string _sceneAssetGUID;

        private GUIContent _errorTooltip;
        private GUIStyle _errorStyle;
        #endregion -- Private Variables ---------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            CacheProperties(property);
            UpdateSceneState();

            position = DisplayErrorsIfNecessary(position);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, _scene, GUIContent.none, false);
            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
                CacheProperties(property);
                UpdateSceneState();
                Validate();
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Cache the used properties as local variables so that they can be used by other methods.
        /// </summary>
        private void CacheProperties(SerializedProperty property) {

            _scene = property.FindPropertyRelative("scene");
            _sceneName = property.FindPropertyRelative("sceneName");
            _sceneIndex = property.FindPropertyRelative("_sceneIndex");
            _sceneEnabled = property.FindPropertyRelative("_sceneEnabled");
            _sceneAsset = (SceneAsset)_scene.objectReferenceValue;

            if (_sceneAsset == null) {
                _sceneAssetPath = null;
                _sceneAssetGUID = null;
            }
            else {
                _sceneAssetPath = AssetDatabase.GetAssetPath(_scene.objectReferenceValue);
                _sceneAssetGUID = AssetDatabase.AssetPathToGUID(_sceneAssetPath);
            }
        }

        /// <summary>
        /// Updates the scene index and enabled flags of a given SceneReference by
        /// checking the editor build settings.
        /// </summary>
        private void UpdateSceneState() {

            if (_sceneAsset != null) {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                _sceneIndex.intValue = -1;
                for (int i = 0; i < scenes.Length; i++) {
                    if (scenes[i].guid.ToString() == _sceneAssetGUID) {
                        if (_sceneIndex.intValue != i)
                            _sceneIndex.intValue = i;
                        _sceneEnabled.boolValue = scenes[i].enabled;
                        if (scenes[i].enabled && _sceneName.stringValue != _sceneAsset.name)
                            _sceneName.stringValue = _sceneAsset.name;
                        break;
                    }
                }

            }
            else {
                _sceneName.stringValue = "";
            }

        }

        /// <summary>
        /// Display a popup error about the selected scene. Fixes the error or opens
        /// the build settings if the user chooses to do so.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void DisplaySceneErrorPrompt(string message) {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int choice = EditorUtility.DisplayDialogComplex("Scene Not In Build", message, "Yes", "No", "Open Build Settings");

            if (choice == 0) {
                EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[_sceneIndex.intValue < 0 ? scenes.Length + 1 : scenes.Length];
                Array.Copy(scenes, newScenes, scenes.Length);

                if (_sceneIndex.intValue < 0) {
                    newScenes[scenes.Length] = new EditorBuildSettingsScene(_sceneAssetPath, true);
                    _sceneIndex.intValue = scenes.Length;
                }
                newScenes[_sceneIndex.intValue].enabled = true;
                EditorBuildSettings.scenes = newScenes;
            }
            else if (choice == 2) {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }

        }

        /// <summary>
        /// Displays an error if there is anything wrong with the selected scene.
        /// </summary>
        /// <param name="position">Rect for drawing the property.</param>
        /// <returns>The rect to draw the rest of the property. If there are no errors,
        /// it will be the same as the input position.</returns>
        private Rect DisplayErrorsIfNecessary(Rect position) {
            if (_errorStyle == null) {
                _errorStyle = "CN EntryErrorIconSmall";
                _errorTooltip = new GUIContent("", "error");
            }

            if (_sceneAsset == null)
                return position;

            Rect warningRect = new Rect(position) {
                width = _errorStyle.fixedWidth + 4
            };

            if (_sceneIndex.intValue < 0) {
                _errorTooltip.tooltip = _TOOLTIP_SCENE_MISSING;
                position.xMin = warningRect.xMax;
                if (GUI.Button(warningRect, _errorTooltip, _errorStyle))
                    DisplaySceneErrorPrompt(_ERROR_SCENE_MISSING);
            }
            else if (!_sceneEnabled.boolValue) {
                _errorTooltip.tooltip = _TOOLTIP_SCENE_DISABLED;
                position.xMin = warningRect.xMax;
                if (GUI.Button(warningRect, _errorTooltip, _errorStyle))
                    DisplaySceneErrorPrompt(_ERROR_SCENE_DISABLED);
            }

            return position;
        }

        /// <summary>
        /// Validate any new values in the scene property, displaying popup errors
        /// if there are issues.
        /// </summary>
        private void Validate() {
            if (_sceneAsset != null) {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                _sceneIndex.intValue = -1;

                for (int i = 0; i < scenes.Length; i++) {
                    if (scenes[i].guid.ToString() == _sceneAssetGUID) {
                        if (_sceneIndex.intValue != i)
                            _sceneIndex.intValue = i;
                        if (scenes[i].enabled) {
                            if (_sceneName.stringValue != _sceneAsset.name)
                                _sceneName.stringValue = _sceneAsset.name;
                            return;
                        }
                        break;
                    }
                }

                if (_sceneIndex.intValue >= 0)
                    DisplaySceneErrorPrompt(_ERROR_SCENE_DISABLED);
                else
                    DisplaySceneErrorPrompt(_ERROR_SCENE_MISSING);
            }
            else {
                _sceneName.stringValue = "";
            }
        }

    }

}