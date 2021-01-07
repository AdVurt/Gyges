using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Game {
    [Serializable]
    public struct QueuedEnemyAction {
        public enum ActionType {
            WaitForSeconds,
            SetVelocity,
            SetRotationSpeed,
            Loop
        };

        public ActionType actionType;
        public int[] intValues;
        public float[] floatValues;
        public Vector2[] vector2Values;
        public UnityEvent onFinished;
    }
}
