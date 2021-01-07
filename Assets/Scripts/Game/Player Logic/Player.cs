using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Gyges.Pooling;

namespace Gyges.Game {

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Ship))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(AudioSource))]
    public class Player : MonoBehaviour {

        [SerializeField] private ProjectileLauncher _frontLauncher = default;
        [SerializeField] private ProjectileLauncher _rearLauncher = default;
        [SerializeField] private Texture2D _targetCursor = default;
        [SerializeField] private GameObject _trailPrefab = default;

        private Camera _mainCam;
        private Collider2D _collider;
        private Animator _animator;
        public Ship Ship { get; private set; }
        private PlayerInput _input;
        private MeshRenderer _renderer;
        private Material[] _materials;
        [SerializeField] private AudioSource _frontWeaponAudioSource = default;
        [SerializeField] private AudioSource _rearWeaponAudioSource = default;

        [SerializeField] private GameState _gameState = default;
        public int playerNumber = 0;
        public float moveSpeed = 20f;

        private float _power = 0f;
        private float _shields = 40f;

        /// <summary>
        /// How long to wait between power drain "ticks" when regenerating shields
        /// </summary>
        private const float _regenPowerDrainTick = 1.0f;

        public Loadout CurrentLoadout {
            get {
                return _gameState.loadouts[playerNumber];
            }
        }

        public float Power {
            get {
                return _power;
            }
            set {
                _power = value;
                onPowerChanged?.Invoke(_power);
            }
        }

        public float Shields {
            get {
                return _shields;
            }
            set {
                _shields = value;
                onShieldsChanged?.Invoke(_shields);
            }
        }
        
        public float Hull {
            get {
                return Ship.Health;
            }
            set {
                Ship.Health = value;
            }
        }

        public float MaxPower { get { return 100f; } }
        public float MaxShields { get { return _gameState.loadouts[playerNumber].shield.shieldMax; } }
        public float ShieldRegenRate { get { return _gameState.loadouts[playerNumber].shield.regenRate; } }
        public float ShieldRegenPowerThreshold { get { return _gameState.loadouts[playerNumber].shield.powerThreshold; } }
        public float ShieldRegenPowerCost { get { return _gameState.loadouts[playerNumber].shield.costPerSecond; } }
        public float StartingHull { get { return _gameState.loadouts[playerNumber].hull.startingHull; } }
        public float PowerRegenRate { get { return _gameState.loadouts[playerNumber].generator.powerGeneration; } }

        private float _frontReloadTime = 0f;
        private float _rearReloadTime = 0f;
        private float _regenTime = 0f;
        private float _regenDelay = 0f;
        private bool _isFiring = false;
        private Vector3 _lastKnownMousePosition;
        private Vector2 _lastKnownInputDirection;
        private int _shipCollisionLayer;
        private float _shipCollisionTimer = 0f;

        //private int _collisionLayerMask;

        public event Action<float> onPowerChanged;
        public event Action<float> onShieldsChanged;
        public ProjectileCollisionEvent tookProjectileToShield;

        //For optimisation reasons
        private int _animGoingLeftHash = Animator.StringToHash("GoingLeft");
        private int _animGoingRightHash = Animator.StringToHash("GoingRight");

        void Awake() {
            //Get the objects that will be referenced in other code repeatedly
            _mainCam = Camera.main;
            _animator = GetComponent<Animator>();
            Ship = GetComponent<Ship>();
            _input = GetComponent<PlayerInput>();
            _collider = GetComponent<Collider2D>();
            _frontWeaponAudioSource = GetComponent<AudioSource>();
            _renderer = GetComponent<MeshRenderer>();
            _materials = _renderer.materials;
            _shipCollisionLayer = LayerMask.NameToLayer("Enemy Ships");


            //Setup object pools and visuals based on loadout
            if (CurrentLoadout.frontWeapon != null) {
                _frontWeaponAudioSource.clip = CurrentLoadout.frontWeapon.fireSound;
                if (CurrentLoadout.frontWeaponLevel == 0) {
                    throw new Exception("Front weapon level cannot be set to zero.");
                }
                foreach (GameObject prefab in CurrentLoadout.frontWeapon.projectilePrefabs) {
                    ObjectPoolManager.PoolSetup(prefab, 5 * CurrentLoadout.frontWeapon.levelStats[CurrentLoadout.frontWeaponLevel - 1].formation.Size);
                }
                
            }
            if (CurrentLoadout.rearWeapon != null) {
                _rearWeaponAudioSource.clip = CurrentLoadout.rearWeapon.fireSound;
                if (CurrentLoadout.rearWeaponLevel == 0) {
                    throw new Exception("Rear weapon level cannot be set to zero.");
                }
                foreach (GameObject prefab in CurrentLoadout.rearWeapon.projectilePrefabs) {
                    ObjectPoolManager.PoolSetup(prefab, 5 * CurrentLoadout.rearWeapon.levelStats[CurrentLoadout.rearWeaponLevel - 1].formation.Size);
                }
                
            }
            foreach(Vector3 trailPosition in CurrentLoadout.hull.trailPositions) {
                GameObject obj = Instantiate(_trailPrefab,transform,false);
                obj.transform.localPosition = trailPosition;
            }

        }

        void Start() {
            Power = 0f;
            Hull = CurrentLoadout.hull.startingHull;
            Shields = CurrentLoadout.shield.shieldMax;
        }

        private void OnEnable() {
            //Subscribe to events
            _input.actions["Fire"].performed += Fire;
            _input.actions["Fire"].canceled += StopFire;
            _input.actions["Screen Position"].performed += UpdateMousePos;
            _input.actions["Movement"].performed += UpdateGamepadStickPos;
            _input.actions["Movement"].canceled += UpdateGamepadStickPos;
            _input.actions["Pause"].performed += Pause;
            _lastKnownMousePosition = transform.position;
            Global.OnPausedChanged += Global_OnPausedChanged;
            Cursor.SetCursor(_targetCursor, new Vector2(_targetCursor.width / 2, _targetCursor.height / 2), CursorMode.Auto);
        }

        void OnCollisionStay2D(Collision2D collision) {

            if ( (collision.collider.gameObject.layer & _shipCollisionLayer) > 0 && _shipCollisionTimer <= 0f) {

                TakeDamage(3f);
                _shipCollisionTimer += 0.1f;

            }

        }

        private void Global_OnPausedChanged(bool value) {
            _animator.enabled = !value;

            if (value) {
                if (_frontWeaponAudioSource.isPlaying) {
                    _frontWeaponAudioSource.Pause();
                }
                if (_rearWeaponAudioSource != null && _rearWeaponAudioSource.isPlaying) {
                    _rearWeaponAudioSource.Pause();
                }
            }
            else {
                _frontWeaponAudioSource.UnPause();
                if (_rearWeaponAudioSource != null)
                    _rearWeaponAudioSource.UnPause();
            }
            
        }

        private void OnDisable() {
            //Unsubscribe from events
            _input.actions["Fire"].performed -= Fire;
            _input.actions["Fire"].canceled -= StopFire;
            _input.actions["Screen Position"].performed -= UpdateMousePos;
            _input.actions["Movement"].performed -= UpdateGamepadStickPos;
            _input.actions["Movement"].canceled -= UpdateGamepadStickPos;
            _input.actions["Pause"].performed -= Pause;
            Global.OnPausedChanged -= Global_OnPausedChanged;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;
        }

        void Pause (InputAction.CallbackContext context) {
            Global.Paused = true;
        }

        void UpdateMousePos (InputAction.CallbackContext context) {
            _lastKnownMousePosition = _mainCam.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _lastKnownMousePosition.z = transform.position.z;
        }

        void UpdateGamepadStickPos (InputAction.CallbackContext context) {
            _lastKnownInputDirection = context.ReadValue<Vector2>();
        }

        void Fire (InputAction.CallbackContext context) {
            _isFiring = true;
        }

        void StopFire (InputAction.CallbackContext context) {
            _isFiring = false;
        }



        void FixedUpdate() {

            Cursor.visible = _input.currentControlScheme == "Mouse";

            if (!Global.enableGameLogic || Global.Paused)
                return;


            Vector3 destinationPosition = transform.position;
            float magnitude = 1f;

            if (_input.currentControlScheme == "Mouse") {
                destinationPosition.x = _lastKnownMousePosition.x;
                destinationPosition.y = _lastKnownMousePosition.y;
            }
            else {
                destinationPosition += (Vector3)_lastKnownInputDirection.normalized;
                magnitude = _lastKnownInputDirection.magnitude;
                
            }

            _animator.SetBool(_animGoingLeftHash, destinationPosition.x < transform.position.x);
            _animator.SetBool(_animGoingRightHash, destinationPosition.x > transform.position.x);

            if (transform.position != destinationPosition) {
                destinationPosition.x = Mathf.Clamp(destinationPosition.x, -10f, 10f);
                destinationPosition.y = Mathf.Clamp(destinationPosition.y, -6f, 6f);
                transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * magnitude * Time.fixedDeltaTime);
            }

        }

        // Update is called once per frame
        void Update() {

            if (!Global.enableGameLogic || Global.Paused)
                return;

            //Fire weapons
            if (_isFiring) {

                //Front weapon
                if (CurrentLoadout.frontWeapon != null) {
                    if (_frontReloadTime <= 0f && CurrentLoadout.frontWeapon.HasEnoughPower(CurrentLoadout.frontWeaponLevel, Power)) {
                        WeaponLogic logic = CurrentLoadout.frontWeapon.levelStats[CurrentLoadout.frontWeaponLevel - 1];
                        Power -= logic.powerCost;
                        _frontReloadTime = logic.reloadTime;
                        _frontLauncher.Fire(CurrentLoadout.frontWeapon.projectilePrefabs, logic.formation, logic.damage);
                        _frontWeaponAudioSource.Play();
                    }
                }

                if (CurrentLoadout.rearWeapon != null) {
                    if (_rearReloadTime <= 0f && CurrentLoadout.rearWeapon.HasEnoughPower(CurrentLoadout.rearWeaponLevel, Power)) {
                        WeaponLogic logic = CurrentLoadout.rearWeapon.levelStats[CurrentLoadout.rearWeaponLevel - 1];
                        Power -= logic.powerCost;
                        _rearReloadTime = logic.reloadTime;
                        _rearLauncher.Fire(CurrentLoadout.rearWeapon.projectilePrefabs, logic.formation, logic.damage);
                        _rearWeaponAudioSource.Play();
                    }
                }
                
            }


            //Gradually regenerate power always
            if (Power < MaxPower) {
                Power += PowerRegenRate * Time.deltaTime;
            }

            //Gradually regenerate shields and drain power if power is at the threshold or greater, and we're after the damage delay
            //Power will be ticked down every second to more visually demonstrate the power drain.
            if (Shields < MaxShields && _regenDelay <= 0f) {

                if (_regenTime <= 0f) {
                    if (Power >= ShieldRegenPowerThreshold && (ShieldRegenPowerCost * _regenPowerDrainTick <= Power)) {
                        Power -= ShieldRegenPowerCost * _regenPowerDrainTick;
                        _regenTime += _regenPowerDrainTick;
                    }
                }

                if (_regenTime > 0f) {
                    Shields += ShieldRegenRate * Time.deltaTime;
                    _regenTime -= Time.deltaTime;
                }

            }
            if (Shields >= MaxShields) {
                Shields = MaxShields;
                _regenTime = 0f;
            }


            if (_regenDelay > 0f) {
                _regenDelay -= Time.deltaTime;
            }
            if (_frontReloadTime > 0f) {
                _frontReloadTime -= Time.deltaTime;
            }
            if (_rearReloadTime > 0f) {
                _rearReloadTime -= Time.deltaTime;
            }
            if (_shipCollisionTimer > 0f) {
                _shipCollisionTimer -= Time.deltaTime;
            }

        }

        public void BulletHit(ProjectileCollision coll) {
            if (Shields > 0f) {
                tookProjectileToShield.Invoke(coll);
            }
            TakeDamage(coll.damage);
        }

        public void TakeDamage(float amount) {
            if (amount <= Shields) {
                Shields -= amount;
            }
            else {
                Hull -= amount - Shields;
                Shields = 0f;
            }
            if (CurrentLoadout.shield != null)
                _regenDelay = CurrentLoadout.shield.regenDelay;
        }
    }

}