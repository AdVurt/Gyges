using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class PlayMusicOnEnable : MonoBehaviour, IWaveObject {

        public AudioClip intro;
        public AudioClip music;
        public float delay = 1f;
        public bool loop = true;

        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        void OnEnable() {
            if (intro != null)
                MusicManager.Play(intro, music, delay, loop);
            else
                MusicManager.Play(music, delay, loop);
        }

        void OnDestroy() {
            onDestroy?.Invoke(new IWaveObjectDestroyEventParams());
        }

    }

}