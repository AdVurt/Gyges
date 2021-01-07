// Based on Ryan Hipple's SceneReference.cs code from the following link:
// https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SceneReference/Code/SceneReference.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gyges.Utility {

    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver {
        
        public class SceneLoadException : Exception {
            public SceneLoadException(string message) : base(message) { }
        }

#if UNITY_EDITOR
        public SceneAsset scene;
#endif

        public string sceneName;
        [SerializeField] private int _sceneIndex = -1;
        [SerializeField] private bool _sceneEnabled;

        private void ValidateScene() {
            if (string.IsNullOrEmpty(sceneName))
                throw new SceneLoadException("No scene specified.");
            if (_sceneIndex < 0)
                throw new SceneLoadException($"Scene {sceneName} is not in the build settings.");
            if (!_sceneEnabled)
                throw new SceneLoadException($"Scene {sceneName} is not enabled in the build settings.");
        }

        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single) {
            ValidateScene();
            SceneManager.LoadScene(sceneName, mode);
        }

        public void OnBeforeSerialize() {
#if UNITY_EDITOR

            if (scene != null) {
                string sceneAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene));

                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                _sceneIndex = -1;
                for (int i = 0; i < scenes.Length; i++) {
                    if (scenes[i].guid.ToString() == sceneAssetGUID) {
                        _sceneIndex = i;
                        _sceneEnabled = scenes[i].enabled;
                        if (_sceneEnabled)
                            sceneName = scene.name;
                    }
                }
            }
            else {
                sceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize() { }

    }

}