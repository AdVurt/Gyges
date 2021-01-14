using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class WaveGroup : MonoBehaviour, IWaveObject {
        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        public float initiationDelay = 0f;
        private bool _initiated = false;

        [SerializeField] private Vector2 _velocity = Vector2.zero;
        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }

        void OnDestroy() {
            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, false, 0));
        }

        void Update() {

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
                if (t.TryGetComponent(out IWaveObject obj)) {
                    if (obj.IsOutOfBounds()) {
                        Vector2 totalVelocity = Velocity + obj.Velocity;

                        if ( (totalVelocity.x > 0f && t.position.x > EnemyActions.borders.xMax) ||
                             (totalVelocity.x < 0f && t.position.x < EnemyActions.borders.xMin) ||
                             (totalVelocity.y > 0f && t.position.y > EnemyActions.borders.yMax) ||
                             (totalVelocity.y < 0f && t.position.y < EnemyActions.borders.yMin)
                            )
                        Destroy(t.gameObject);
                    }
                }

            }

            if (transform.childCount == 0)
                Destroy(gameObject);

            
            else {
                transform.position += (Vector3)Velocity * Time.deltaTime;
            }

        }

        public bool IsOutOfBounds() => false;
        public Transform GetTransform() => transform;
    }

}