using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Gyges.Pooling;

namespace Gyges.Game {

    public class ShopShipPreview : MonoBehaviour {

        [SerializeField] private GameState _state = default;
        [SerializeField] private GameObject _trailPrefab = default;
        [SerializeField] private Image _powerBarImage = default;

        private int _playerNumber = -1;
        private MeshFilter _filter;
        private MeshRenderer _renderer;
        private Transform[] _trails = new Transform[0];

        // To help fire demo projectiles
        private Vector3 _frontLaunchPosition = new Vector3(0f, 0.6f, 0f);
        private Vector3 _rearLaunchPosition = Vector3.zero;
        private ProjectileLauncher _frontLauncher;
        private ProjectileLauncher _rearLauncher;
        // Give a second of dead animation time at the start to prevent "catch-up" frames caused by the scene loading.
        private float _startAnimationTimer = 1f;
        private float _frontTimer = 0f;
        private float _rearTimer = 0f;
        private FrontWeapon _frontWeapon;
        private int _frontWeaponLevel = 1;
        private RearWeapon _rearWeapon;
        private int _rearWeaponLevel = 1;
        private Generator _generator;
        private float _power = 0f;

        private bool _altFire = false;
        public bool movingOut = false;
        private float _moveSpeed = 0f;

        /// <summary>
        /// Has this preview ever been refreshed?
        /// </summary>
        private bool _refreshedEver = false;

        public event Action onChanged;

        public int PlayerNumber {
            get {
                return _playerNumber;
            }
            set {
                _playerNumber = value;
                onChanged?.Invoke();
            }
        }

        void Awake() {
            _filter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
            onChanged += Refresh;
        }

        void Start() {
            transform.SetParent(null);
            transform.localScale = Vector3.one;

            if (_frontLauncher == null) {
                _frontLauncher = gameObject.AddComponent<ProjectileLauncher>();
                _frontLauncher.originOffset = _frontLaunchPosition;
            }

            if (_rearLauncher == null) {
                _rearLauncher = gameObject.AddComponent<ProjectileLauncher>();
                _rearLauncher.originOffset = _rearLaunchPosition;
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_refreshedEver) {
                Refresh();
            }

            if (_generator != null) {
                _power = Mathf.Clamp(_power + _generator.powerGeneration * Time.deltaTime , 0f, 100f);
            }

            if (!movingOut) {

                if (_startAnimationTimer >= 0f) {
                    _startAnimationTimer -= Time.deltaTime;
                }
                else {

                    if (_frontWeapon != null) {
                        _frontTimer -= Time.deltaTime;
                        if (_frontWeapon.weaponType == WeaponLogic.WeaponType.OscillatingLaunch) {
                            _frontLauncher.oscillationSpeed = _frontWeapon.oscillationSpeed;
                        }
                        else {
                            _frontLauncher.oscillationSpeed = 0f;
                            _frontLauncher.oscillatedAmount = 0f;
                        }
                        if (_frontTimer <= 0f) {

                            if (_frontWeapon.HasEnoughPower(_frontWeaponLevel, _power)) {
                                WeaponLogic logic = _frontWeapon.levelStats[_frontWeaponLevel - 1];
                                _frontLauncher.Fire(_frontWeapon.projectilePrefabs, logic.formation, logic.damage);
                                _frontTimer += logic.reloadTime;
                                _power -= logic.powerCost;
                            }
                            else {
                                _frontTimer = 0f;
                            }
                            
                        }
                    }

                    if (_rearWeapon != null) {
                        _rearTimer -= Time.deltaTime;
                        WeaponLogic[] stats = _altFire ? _rearWeapon.altLevelStats : _rearWeapon.levelStats;

                        if (_rearWeapon.weaponType == WeaponLogic.WeaponType.OscillatingLaunch) {
                            _rearLauncher.oscillationSpeed = _rearWeapon.oscillationSpeed;
                        }
                        else {
                            _rearLauncher.oscillationSpeed = 0f;
                            _rearLauncher.oscillatedAmount = 0f;
                        }
                        _rearLauncher.oscillationSpeed = _rearWeapon.weaponType == WeaponLogic.WeaponType.OscillatingLaunch ? _rearWeapon.oscillationSpeed : 0f;
                        if (_rearTimer <= 0f) {
                            if (_rearWeapon.HasEnoughPower(_rearWeaponLevel, _power, _altFire)) {
                                WeaponLogic logic = stats[_rearWeaponLevel - 1];
                                _rearLauncher.Fire(_rearWeapon.projectilePrefabs, logic.formation, logic.damage);
                                _rearTimer += logic.reloadTime;
                                _power -= logic.powerCost;
                            }
                            else {
                                _rearTimer = 0f;
                            }
                        }
                    }

                }

            }
            else { //If we're transitioning ahead to the next level...

                _moveSpeed += Time.deltaTime * 10f;
                transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

            }

            _powerBarImage.fillAmount = _power / 100f;
        }

        /// <summary>
        /// Update the visual representation to match the player's loadout.
        /// </summary>
        public void Refresh() {
            if (_playerNumber == -1)
                return;

            Hull loadoutHull = _state.loadouts[_playerNumber].hull;
            _filter.sharedMesh = loadoutHull.shipModel;
            _renderer.sharedMaterials = loadoutHull.modelMaterials;
            _renderer.enabled = true;

            bool trailsMatch = loadoutHull.trailPositions.Length == _trails.Length;

            for (int i = 0; i < loadoutHull.trailPositions.Length && trailsMatch; i++) {
                if (loadoutHull.trailPositions[i] != _trails[i].localPosition) {
                    trailsMatch = false;
                }
            }
            if (!trailsMatch) {
                Transform[] newTrails = new Transform[loadoutHull.trailPositions.Length];
                //If we don't have enough trails, instantiate new ones.
                if (loadoutHull.trailPositions.Length > _trails.Length) {
                    for (int i = 0; i < newTrails.Length; i++) {
                        newTrails[i] = i < _trails.Length ? _trails[i] : Instantiate(_trailPrefab,transform,false).GetComponent<Transform>();
                    }
                }
                //If we have too many trails, destroy the excess ones.
                else if (loadoutHull.trailPositions.Length < _trails.Length) {
                    for (int i = 0; i < _trails.Length; i++) {
                        if (i < newTrails.Length)
                            newTrails[i] = _trails[i];
                        else
                            Destroy(_trails[i].gameObject);
                    }
                }
                //Then, reposition all trails to match.
                for (int i = 0; i < newTrails.Length; i++) {
                    newTrails[i].localPosition = loadoutHull.trailPositions[i];
                }
                _trails = newTrails;
            }

            _frontWeapon = _state.loadouts[_playerNumber].frontWeapon;
            _frontWeaponLevel = _state.loadouts[_playerNumber].frontWeaponLevel;
            if (_frontWeapon != null) {
                foreach (GameObject prefab in _frontWeapon.projectilePrefabs) {
                    ObjectPoolManager.PoolSetup(prefab, 10);
                }
            }

            _rearWeapon = _state.loadouts[_playerNumber].rearWeapon;
            _rearWeaponLevel = _state.loadouts[_playerNumber].rearWeaponLevel;
            if (_rearWeapon != null) {
                foreach (GameObject prefab in _rearWeapon.projectilePrefabs) {
                    ObjectPoolManager.PoolSetup(prefab, 10);
                }
            }

            _generator = _state.loadouts[_playerNumber].generator;

            _refreshedEver = true;
        }

        public void SetAltFireMode(bool value) {
            _altFire = value;
            Refresh();
        }

    }

}