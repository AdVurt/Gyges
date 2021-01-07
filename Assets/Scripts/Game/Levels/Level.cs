using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Utility;

namespace Gyges.Game {

    [CreateAssetMenu(fileName = "New Level", menuName = "Gyges/Level")]
    public class Level : ScriptableObject {

        public SceneReference scene;
        public string inGameName = "";
        public AudioClip startingMusic;

        public void LoadLevel() {
            scene.LoadScene();
        }
    }

}