using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Gyges.Utility;
using UnityEditor;

namespace Gyges.Game {
    /// <summary>
    /// Parent class for "enemy action" classes to inherit from.
    /// In order to function when inheriting from this class, OnEnable should be implemented.
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public abstract class EnemyActions : MonoBehaviour {

        public float timeMultiplier = 1f;
        private Enemy _enemy;
        protected Rect _borders;
        private Vector2 _velocity = new Vector2(0f, 0f);
        private float _rotationSpeed = 0f;
        [SerializeField] protected QueuedEnemyAction[] _actions = new QueuedEnemyAction[0];
        protected Queue<QueuedEnemyAction> _actionQueue;

        public bool selfDestructWhenDone = false;
        public UnityEvent onFinished;

        public void SetRotationSpeed(float value) {
            _rotationSpeed = value;
        }

        public float GetRotationSpeed() {
            return _rotationSpeed;
        }

        public void SetVelocity(Vector2 value) {
            _velocity = value;
        }

        public int GetActionCount() {
            return _actions.Length;
        }

        public Vector2 GetVelocity() {
            return _velocity;
        }

        protected void Awake() {
            _enemy = GetComponent<Enemy>();
            _borders = Projectile.borders;
            _actionQueue = new Queue<QueuedEnemyAction>(_actions);
            if (selfDestructWhenDone) {
                onFinished.AddListener(SelfDestructImmediate);
            }
        }

        protected abstract void OnEnable();

        protected void Update() {

            if (_rotationSpeed != 0f) {
                _velocity = _velocity.Rotate(_rotationSpeed * Time.deltaTime * timeMultiplier);
            }
            transform.position += (Vector3)_velocity * Time.deltaTime * timeMultiplier;

            //If we're out of actions and we're outside of the game borders, self destruct
            if (_actionQueue.Count == 0 && (
                (_velocity.x > 0f && transform.position.x > _borders.xMax) ||
                (_velocity.x < 0f && transform.position.x < _borders.xMin) ||
                (_velocity.y > 0f && transform.position.y > _borders.yMax) ||
                (_velocity.y < 0f && transform.position.y < _borders.yMin)
                )) {
                SelfDestructImmediate();
            }
        }

        /// <summary>
        /// Changes the current velocity to a given speed over a given period of time.
        /// If that period is 0, the change will be instant.
        /// </summary>
        /// <param name="value">The new velocity.</param>
        /// <param name="fadeTime">The length of time (in seconds) to take to gradually reach the new velocity.</param>
        protected IEnumerator SetVelocity(Vector2 value, float fadeTime) {
            yield return ChangeOverTime(_velocity, value, SetVelocity, Vector2.Lerp, fadeTime * (1f/timeMultiplier));
        }

        /// <summary>
        /// Changes the current rotation speed to a given speed over a given period of time.
        /// If that period is 0, the change will be instant.
        /// </summary>
        /// <param name="value">The new rotation speed.</param>
        /// <param name="fadeTime"></param>
        /// <returns></returns>
        protected IEnumerator SetRotationSpeed(float value, float fadeTime) {
            yield return ChangeOverTime(_rotationSpeed, value, SetRotationSpeed, Mathf.Lerp, fadeTime * (1f / timeMultiplier));
        }

        /// <summary>
        /// Waits for a specified amount of time.
        /// </summary>
        protected IEnumerator Wait(float seconds) {
            yield return new WaitForSeconds(seconds * (1f / timeMultiplier));
        }

        /// <summary>
        /// Resets the action queue to its initial state.
        /// </summary>
        protected IEnumerator Loop() {
            _actionQueue = new Queue<QueuedEnemyAction>(_actions);
            yield break;
        }

        protected IEnumerator ExecuteCoRt(QueuedEnemyAction action) {
            switch (action.actionType) {
                case QueuedEnemyAction.ActionType.WaitForSeconds:
                    yield return Wait(action.floatValues[0]);
                    break;
                case QueuedEnemyAction.ActionType.SetVelocity:
                    yield return SetVelocity(action.vector2Values[0], action.floatValues[0]);
                    break;
                case QueuedEnemyAction.ActionType.SetRotationSpeed:
                    yield return SetRotationSpeed(action.floatValues[0], action.floatValues[1]);
                    break;
                case QueuedEnemyAction.ActionType.Loop:
                    yield return Loop();
                    break;
                default:
                    throw new ArgumentException("Unknown action type");
            }
        }

        protected void Finished() {
            onFinished?.Invoke();
        }

        public void SelfDestructImmediate() {
            _enemy.Kill(false);
        }

#if UNITY_EDITOR
        protected abstract void OnDrawGizmosSelected();

        protected static void DrawString(string text, Vector3 worldPos, Color? colour = null) {
            Handles.BeginGUI();

            var restoreColor = GUI.color;

            if (colour.HasValue)
                GUI.color = colour.Value;
            var view = SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0) {
                GUI.color = restoreColor;
                Handles.EndGUI();
                return;
            }

            GUIStyle st = new GUIStyle(EditorStyles.boldLabel) {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            Vector2 size = st.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, st);
            GUI.color = restoreColor;
            Handles.EndGUI();
        }
#endif

        /// <summary>
        /// Helper/parent function for the "set value to given value linearly interpolated over time" coroutines.
        /// </summary>
        /// <param name="startingValue">The current value when this coroutine is first called.</param>
        /// <param name="value">The desired value after fadeTime seconds.</param>
        /// <param name="setter">Setter method for the field being updated.</param>
        /// <param name="lerpFunc">A function that can linearly interpolate between two values of type T.</param>
        /// <param name="fadeTime">The length of time to fade the starting value into the new value.</param>
        private IEnumerator ChangeOverTime<T>(T startingValue, T value, Action<T> setter, Func<T, T, float, T> lerpFunc, float fadeTime) {
            if (fadeTime == 0f) {
                setter(value);
                yield break;
            }

            float counter = 0f;
            while (counter < fadeTime) {
                counter += Time.deltaTime;
                setter(lerpFunc(startingValue, value, counter / fadeTime));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}