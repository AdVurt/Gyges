using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class LocalPositionLerper : MonoBehaviour {

        private Transform _transform;

        public Vector2 minValue = Vector2.zero;
        public Vector2 maxValue = Vector2.right;
        public float totalLoopTime = 1f;

        private float _timeTaken = 0f;

        void Awake() {
            _transform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update() {
            _timeTaken += Time.deltaTime;
            if (_timeTaken > totalLoopTime)
                _timeTaken -= totalLoopTime;


            if (_timeTaken > totalLoopTime / 2f) {
                _transform.localPosition = Vector3.Lerp(maxValue, minValue, (_timeTaken - totalLoopTime/2f) / totalLoopTime );
            }
            else {
                _transform.localPosition = Vector3.Lerp(minValue, maxValue, _timeTaken / (totalLoopTime / 2) );
            }
        }

    }

}