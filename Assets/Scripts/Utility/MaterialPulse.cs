using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Game {

    public class MaterialPulse : MonoBehaviour {


        [Header("Material")]
        [SerializeField] private Renderer _renderer = default;
        [SerializeField] private int _materialNumber = default;
        [SerializeField] private string _materialProperty = default;
        public float minValue = 0f;
        public float maxValue = 1f;

        [Header("Pulse Logic")]
        public float firstPulseDelay = 0f;
        public float inactiveTime = 0.5f;
        public float upTime = 0.5f;
        public float downTime = 0.5f;
        private float _timer = 0f;
        private int _phase = -1;


        [Header("Game Logic")]
        public UnityEvent onMaxPulse;
        public UnityEvent onMinPulse;

        private Material _material;
        private int _materialPropHash;

        private void Awake() {
            _material = _renderer.materials[_materialNumber];
            _materialPropHash = Shader.PropertyToID(_materialProperty);

            _material.SetFloat(_materialPropHash, minValue);

            //If there is no delay before the first pulse, skip the "pre-pulse" phase.
            if (firstPulseDelay == 0f)
                _phase = 0;
        }

        void Update() {

            _timer += Time.deltaTime;

            switch (_phase) {
                case -1:
                    if (_timer >= firstPulseDelay) {
                        _timer -= firstPulseDelay;
                        _phase++;
                    }
                    break;

                case 0:
                    if (_timer >= inactiveTime) {
                        _timer -= inactiveTime;
                        _phase++;
                    }
                    break;

                case 1:
                    _material.SetFloat(_materialPropHash, Mathf.Lerp(minValue, maxValue,  _timer / upTime) );
                    if (_timer >= upTime) {
                        onMaxPulse.Invoke();
                        _timer -= upTime;
                        _phase++;
                    }
                    break;

                case 2:

                    _material.SetFloat(_materialPropHash, Mathf.Lerp(maxValue, minValue, _timer / downTime));
                    if (_timer >= downTime) {
                        _material.SetFloat(_materialPropHash, minValue);
                        onMinPulse.Invoke();
                        _timer -= downTime;
                        _phase = 0;
                    }
                    break;
            }

        }


    }

}