using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class ControllerWaveObject : MonoBehaviour, IWaveObject {

        public bool StartLogicBeforeGameplay => false;
        private bool _dead = false;

        public Vector2 Velocity {
            get {
                Vector2 result = Vector2.zero;
                foreach (IVelocitySetter vel in GetComponents<IVelocitySetter>())
                    result += vel.GetVelocity();
                return result;
            }
        }

        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        public void Kill(bool killedByPlayer = false) {
            if (_dead)
                return;

            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, killedByPlayer, 0));
            _dead = true;
            Destroy(gameObject);
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool IsOutOfBounds() => IsOutOfBounds(out Enemy.OutOfBoundsDirections dummy);

        public bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir) {
            Enemy.OutOfBoundsDirections outres = Enemy.OutOfBoundsDirections.None;

            if (transform.position.x > EnemyActions.borders.xMax)
                outres |= Enemy.OutOfBoundsDirections.East;
            else if (transform.position.x < EnemyActions.borders.xMin)
                outres |= Enemy.OutOfBoundsDirections.West;

            if (transform.position.y > EnemyActions.borders.yMax)
                outres |= Enemy.OutOfBoundsDirections.North;
            else if (transform.position.y < EnemyActions.borders.yMin)
                outres |= Enemy.OutOfBoundsDirections.South;

            oobDir = outres;
            return outres != Enemy.OutOfBoundsDirections.None;
        }

    }

}