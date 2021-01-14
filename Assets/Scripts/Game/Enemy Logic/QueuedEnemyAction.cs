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
            SetPosition,
            UnityEvent,
            Loop
        };

        public ActionType actionType;
        public int[] intValues;
        public float[] floatValues;
        public Vector2[] vector2Values;
        public bool[] boolValues;
        public UnityEvent eventValue;
        public UnityEvent onFinished;

        public void InvokeEvent() {
            eventValue?.Invoke();
        }
    }
}
