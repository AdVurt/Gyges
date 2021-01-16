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
        public bool IsOutOfBounds() => IsOutOfBounds(out Enemy.OutOfBoundsDirections dummy);

        public bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir) {
            Enemy.OutOfBoundsDirections outResult = Enemy.OutOfBoundsDirections.None;

            if (transform.position.x > EnemyActions.borders.xMax)
                outResult |= Enemy.OutOfBoundsDirections.East;
            else if (transform.position.x < EnemyActions.borders.xMin)
                outResult |= Enemy.OutOfBoundsDirections.West;

            if (transform.position.y > EnemyActions.borders.yMax)
                outResult |= Enemy.OutOfBoundsDirections.North;
            else if (transform.position.y < EnemyActions.borders.yMin)
                outResult |= Enemy.OutOfBoundsDirections.South;

            oobDir = outResult;
            return oobDir != Enemy.OutOfBoundsDirections.None;
        }

        public Transform GetTransform() => transform;
        public bool StartLogicBeforeGameplay { get => false; }
        #endregion
    }

}