using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {
    public class ProjectileManager : MonoBehaviour {
        
        public static ProjectileManager Instance { get; private set; }

        public event Action OnUpdate;

        void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            Instance = null;
        }

        void Update() {
            if (!Global.Paused)
                OnUpdate?.Invoke();
        }
    }

    [Serializable]
    public struct ProjectileFormation : IEnumerable, IEquatable<ProjectileFormation> {

        /// <summary>
        /// Returns a formation consisting of a single projectile moving exactly forwards.
        /// </summary>
        public static ProjectileFormation SingleStraight {
            get {
                return new ProjectileFormation(ProjectileLocation.Zero);
            }
        }

        public int Size {
            get {
                return _locs.Length;
            }
        }

        [SerializeField]
        private ProjectileLocation[] _locs;
#if UNITY_EDITOR
        /// <summary>
        /// Used only in the custom property drawer.
        /// </summary>
        [HideInInspector] public int selectedProjectile;
        public int SelectedProjectile {
            get {
                return selectedProjectile;
            }
        }
        [HideInInspector] public string guid;
#endif


        public ProjectileLocation this[int index] {
            get { return _locs[index]; }
        }

        public IEnumerator GetEnumerator() {
            return _locs.GetEnumerator();
        }

        public ProjectileFormation(params ProjectileLocation[] locations) {
            _locs = locations;
#if UNITY_EDITOR
            selectedProjectile = -1;
            guid = Guid.NewGuid().ToString();
#endif
        }


        public bool Equals(ProjectileFormation other) => _locs == other._locs;
        public override bool Equals(object obj) => Equals((ProjectileFormation)obj);
        public override int GetHashCode() => _locs.GetHashCode();

        public static bool operator ==(ProjectileFormation a, ProjectileFormation b) => a._locs == b._locs;
        public static bool operator !=(ProjectileFormation a, ProjectileFormation b) => a._locs != b._locs;
    }


    [Serializable]
    public struct ProjectileLocation {

        private static readonly Vector2 _defaultSpeed = new Vector2(0f, 15f);
        public static Vector2 DefaultSpeed {
            get {
                return _defaultSpeed;
            }
        }

        /// <summary>
        /// Returns a centralised projectile location with no offset or rotation applied to it.
        /// </summary>
        public static ProjectileLocation Zero {
            get {
                return new ProjectileLocation(0f,0f);
            }
        }

        public float x;
        public float y;
        public Vector2 speed;

        /// <summary>
        /// Rotation as expressed in degrees.
        /// The game is 2D, so we only need one value.
        /// </summary>
        public float rotation;
        public float scale;
        /// <summary>
        /// How much  damage should this do compared to a normal projectile in this formation?
        /// </summary>
        public float damageMultiplier;

        /// <summary>
        /// The prefab associated with this projectile. Intended to line up with the LevelableWeapon's array of prefabs.
        /// </summary>
        public int prefabToUse;


        public ProjectileLocation(float x, float y, Vector2 speed, float rotation, float scale = 1f, int prefabToUse = 0, float damageMultiplier = 1f) {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.rotation = rotation;
            this.scale = scale;
            this.prefabToUse = prefabToUse;
            this.damageMultiplier = damageMultiplier;
        }

        public override string ToString() {
            return $"Position: ({x},{y}), Speed: ({speed.x},{speed.y}), Rotation: {rotation}, Scale: {scale}, Prefab: {prefabToUse}, Dmg Multi: {damageMultiplier}";
        }

        public ProjectileLocation(float x, float y, Vector2 speed) : this (x, y, speed, 0f) { }
        public ProjectileLocation(float x, float y) : this(x, y, DefaultSpeed, 0f) { }
        public ProjectileLocation(Vector2 position, Vector2 speed, float rotation) : this(position.x, position.y, speed, rotation) { }
        public ProjectileLocation(Vector2 position, Vector2 speed) : this(position, speed, 0f) { }
        public ProjectileLocation(Vector2 position) : this(position.x, position.y) { }
    }
}