using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Utility;

namespace Gyges.Game {

    public class MaterialConstantPulse : MonoBehaviour {

        
        [ReadOnly(true), SerializeField] private Renderer _renderer = default;
        [ReadOnly(true), SerializeField] private int _materialNumber = default;
        [ReadOnly(true), SerializeField] private string _internalFieldName = default;

        [Space, SerializeField] private Color _value1 = default;
        [SerializeField] private Color _value2 = default;
        [SerializeField,Tooltip("The time it takes to go from Value 1 to Value 2, or vice versa.")]
        private float _lerpTime = default;

        private Material _material;
        private int _internalField;

        private float _lerp = 0f;

        void Awake() {

            _material = _renderer.materials[_materialNumber];
            _internalField = Shader.PropertyToID(_internalFieldName);

        }


        // Update is called once per frame
        void Update() {

            _lerp += Time.deltaTime / _lerpTime;
            while (_lerp > 2f)
                _lerp -= 2f;

            _material.SetColor(_internalField, Color.Lerp(_value1, _value2, _lerp > 1f ? (2f - _lerp) : _lerp));

        }
    }

}