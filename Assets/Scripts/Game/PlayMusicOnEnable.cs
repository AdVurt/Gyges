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
            Destroy(gameObject);
        }

        void OnDestroy() {
            onDestroy?.Invoke(new IWaveObjectDestroyEventParams());
        }


        #region Interface Members
        public Vector2 Velocity => Vector2.zero;
        public bool IsOutOfBounds() => !EnemyActions.borders.Contains(transform.position);
        public Transform GetTransform() => transform;
        #endregion
    }

}