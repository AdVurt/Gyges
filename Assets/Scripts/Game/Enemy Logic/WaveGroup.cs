using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class WaveGroup : MonoBehaviour, IWaveObject {
        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        public float initiationDelay = 0f;
        private bool _initiated = false;
        public bool Initiated { get => _initiated; }

        [SerializeField] private Vector2 _velocity = Vector2.zero;
        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }

        void OnDestroy() {
            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, false, 0));
        }

        void Update() {

            if (Global.Paused || !Global.enableGameLogic)
                return;


            if (!_initiated) {
                initiationDelay -= Time.deltaTime;
                if (initiationDelay <= 0f) {
                    foreach (Transform t in transform) {
                        if (t == transform)
                            continue;
                        t.gameObject.SetActive(true);
                    }
                    _initiated = true;
                }
                else
                    return;
            }


            //Destroy children who are out of bounds
            foreach(Transform t in transform) {
                if (t == transform)
                    continue;

                bool outofBounds = false;
                Vector2 totalVelocity = Velocity;
                Enemy.OutOfBoundsDirections outOfBoundsDirections = Enemy.OutOfBoundsDirections.None;

                foreach (IWaveObject obj in t.GetComponents<IWaveObject>()) {
                    totalVelocity += obj.Velocity;
                    if (obj.IsOutOfBounds(out Enemy.OutOfBoundsDirections dir)) {
                        outofBounds = true;
                        outOfBoundsDirections |= dir;
                    }
                }

                if (!outofBounds)
                    continue;

                if ((totalVelocity.x > 0f && (outOfBoundsDirections & Enemy.OutOfBoundsDirections.East) > 0) ||
                    (totalVelocity.x < 0f && (outOfBoundsDirections & Enemy.OutOfBoundsDirections.West) > 0) ||
                    (totalVelocity.y > 0f && (outOfBoundsDirections & Enemy.OutOfBoundsDirections.North) > 0) ||
                    (totalVelocity.y < 0f && (outOfBoundsDirections & Enemy.OutOfBoundsDirections.South) > 0)
                    )
                    Destroy(t.gameObject);
                }

            if (transform.childCount == 0)
                Destroy(gameObject);

            
            else {
                transform.position += (Vector3)Velocity * Time.deltaTime;
            }

        }

        public bool StartLogicBeforeGameplay { get => false; }
        public bool IsOutOfBounds() => false;
        public bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir) {
            oobDir = Enemy.OutOfBoundsDirections.None;
            return false;
        }
        public Transform GetTransform() => transform;
    }

}