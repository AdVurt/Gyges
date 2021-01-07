using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {
    [RequireComponent(typeof(Loadout))]
    public class ShipShieldController : MonoBehaviour {
        private const int MAX_HITS_COUNT = 10;
        [SerializeField] Renderer _renderer = default;
        [SerializeField] private Player _player = default;
        [SerializeField] private float _hitBorder = 0.1f;
        [SerializeField] private float _hitDuration = 0.1f;
        [SerializeField] private float _hitRadius = 2f;
        MaterialPropertyBlock _mpb;
        int _hitsCount;
        private Transform _transform;
        private float _shieldValue = 100f;
        private float _shieldAlpha = 1f;

        // In order to optimise the shader parameter calls
        private readonly int _alphaMultiplierMatFloat = Shader.PropertyToID("_AlphaMultiplier");
        private readonly int _dissolveValueMatFloat = Shader.PropertyToID("_DissolveValue");
        private readonly int _hitsCountMatFloat = Shader.PropertyToID("_HitsCount");
        private readonly int _hitsRadiusMatFloatArr = Shader.PropertyToID("_HitsRadius");
        private readonly int _hitsObjPosMatVector = Shader.PropertyToID("_HitsObjectPosition");
        private readonly int _hitsIntensityMatFloat = Shader.PropertyToID("_HitsIntensity");
        private readonly int _borderMatFloat = Shader.PropertyToID("_Border");

        Vector4[] _hitsObjectPosition = new Vector4[MAX_HITS_COUNT];
        float[] _hitsDuration = new float[MAX_HITS_COUNT];
        float[] _hitsTimer = new float[MAX_HITS_COUNT];
        float[] _hitsRadius = new float[MAX_HITS_COUNT];
        float[] _hitsIntensity = new float[MAX_HITS_COUNT];

        public void AddHitProjColl(ProjectileCollision coll) {
            AddHit(coll.position, _hitDuration, _hitRadius);
        }

        public void AddHit(Vector3 worldPosition, float duration, float radius) {
            int id = GetFreeHitId();
            _hitsObjectPosition[id] = transform.InverseTransformPoint(worldPosition);
            _hitsDuration[id] = duration;
            _hitsRadius[id] = radius;
            _hitsTimer[id] = 0;
        }

        int GetFreeHitId() {
            if (_hitsCount < MAX_HITS_COUNT) {
                _hitsCount++;
                return _hitsCount - 1;
            }
            else {
                float minDuration = float.MaxValue;
                int minId = 0;
                for (int i = 0; i < MAX_HITS_COUNT; i++) {
                    if (_hitsDuration[i] < minDuration) {
                        minDuration = _hitsDuration[i];
                        minId = i;
                    }
                }
                return minId;
            }
        }

        void Awake() {
            _mpb = new MaterialPropertyBlock();
            _transform = transform;
        }

        void OnEnable() {
            if (_player != null) {
                _player.onShieldsChanged += SetShield;
            }
        }

        void OnDisable() {
            if (_player != null) {
                _player.onShieldsChanged -= SetShield;
            }
        }

        public void ClearAllHits() {
            _hitsCount = 0;
            SendHitsToRenderer();
        }

        void Update() {
            if (Global.Paused)
                return;

            _shieldAlpha = Mathf.MoveTowards(_shieldAlpha,_shieldValue > 0f ? 1f : 0f, Time.deltaTime * 10f);

            UpdateHitsLifeTime();
            SendHitsToRenderer();
        }

        public void SetShield(float val) {
            _shieldValue = val;
        }

        void UpdateHitsLifeTime() {
            for (int i = 0; i < _hitsCount;) {
                _hitsTimer[i] += Time.deltaTime;
                if (_hitsTimer[i] > _hitsDuration[i]) {
                    SwapWithLast(i);
                }
                else {
                    i++;
                }
            }
        }

        void SwapWithLast(int id) {
            int idLast = _hitsCount - 1;
            if (id != idLast) {
                _hitsObjectPosition[id] = _hitsObjectPosition[idLast];
                _hitsDuration[id] = _hitsDuration[idLast];
                _hitsTimer[id] = _hitsTimer[idLast];
                _hitsRadius[id] = _hitsRadius[idLast];
            }
            _hitsCount--;
        }

        void SendHitsToRenderer() {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(_alphaMultiplierMatFloat, _shieldAlpha);
            _mpb.SetFloat(_hitsCountMatFloat, _hitsCount);
            _mpb.SetFloatArray(_hitsRadiusMatFloatArr, _hitsRadius);
            _mpb.SetFloat(_borderMatFloat, _hitBorder);
            for (int i = 0; i < _hitsCount; i++) {
                if (_hitsDuration[i] > 0f) {
                    _hitsIntensity[i] = 1 - Mathf.Clamp01(_hitsTimer[i] / _hitsDuration[i]);
                }
            }
            _mpb.SetVectorArray(_hitsObjPosMatVector, _hitsObjectPosition);
            _mpb.SetFloatArray(_hitsIntensityMatFloat, _hitsIntensity);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}