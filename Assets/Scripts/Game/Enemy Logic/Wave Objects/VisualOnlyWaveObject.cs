using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {

    public class VisualOnlyWaveObject : MonoBehaviour, IWaveObject {

        private bool _dead = false;
        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        [SerializeField] private bool _startLogicBeforeGameplay = false;
        public bool StartLogicBeforeGameplay { get => _startLogicBeforeGameplay; }

        [SerializeField] private Vector2 _velocity = Vector2.zero;
        public Vector2 Velocity {
            get {
                return _velocity;
            }
            set {
                _velocity = value;
            }
        }


        private Vector3 _meshExtents;

        void Awake() {
            _meshExtents = Vector3.Scale(GetComponent<MeshFilter>().sharedMesh.bounds.extents , transform.lossyScale);
        }

        void Update() {

            if (Global.Paused)
                return;

            transform.position += (Vector3)_velocity * Time.deltaTime;

            if (
                (_velocity.x > 0f && transform.position.x - _meshExtents.x > EnemyActions.borders.xMax) ||
                (_velocity.x < 0f && transform.position.x + _meshExtents.x < EnemyActions.borders.xMin) ||
                (_velocity.y > 0f && transform.position.y - _meshExtents.y > EnemyActions.borders.yMax) ||
                (_velocity.y < 0f && transform.position.y + _meshExtents.y < EnemyActions.borders.yMin)
                ) {
                Kill();
            }

        }

        #region Interface required methods
        public bool IsOutOfBounds() => IsOutOfBounds(out Enemy.OutOfBoundsDirections dummy);

        public bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir) {
            Enemy.OutOfBoundsDirections outres = Enemy.OutOfBoundsDirections.None;

            if (transform.position.x - _meshExtents.x > EnemyActions.borders.xMax)
                outres |= Enemy.OutOfBoundsDirections.East;
            else if (transform.position.x + _meshExtents.x < EnemyActions.borders.xMin)
                outres |= Enemy.OutOfBoundsDirections.West;

            if (transform.position.y - _meshExtents.y > EnemyActions.borders.yMax)
                outres |= Enemy.OutOfBoundsDirections.North;
            else if (transform.position.y + _meshExtents.y < EnemyActions.borders.yMin)
                outres |= Enemy.OutOfBoundsDirections.South;

            oobDir = outres;
            return outres != Enemy.OutOfBoundsDirections.None;
        }
        public Transform GetTransform() => transform;

        public void Kill(bool killedByPlayer = false) {
            if (_dead)
                return;

            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, false, 0));
            Destroy(gameObject);
            _dead = true;
        }
        #endregion

    }

}