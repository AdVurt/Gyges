using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class PlayMusicOnEnable : MonoBehaviour {

        public AudioClip music;
        public float delay = 1f;

        void OnEnable() {
            MusicManager.Play(music, delay);
        }

    }

}