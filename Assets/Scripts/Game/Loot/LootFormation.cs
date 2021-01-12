using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {

    [Serializable]
    public class LootFormation : IEnumerable, IEquatable<LootFormation> {


        [SerializeField]
        private LootLocation[] _locs = new LootLocation[0];
#if UNITY_EDITOR
        /// <summary>
        /// Used only in the custom property drawer.
        /// </summary>
        [HideInInspector] public int selectedLoot;
        [HideInInspector] public string guid;
#endif

        public LootLocation this[int index] {
            get { return _locs[index]; }
            set { _locs[index] = value; }
        }

        public int Count {
            get {
                return _locs.Length;
            }
        }

        public IEnumerator GetEnumerator() => _locs.GetEnumerator();

        public LootFormation(params LootLocation[] locations) {
            _locs = locations;
        }

        public bool Equals(LootFormation other) => _locs == other._locs;
        public override bool Equals(object obj) => Equals((LootFormation)obj);
        public override int GetHashCode() => (_locs.GetHashCode() + 1).GetHashCode();

        public static bool operator ==(LootFormation a, LootFormation b) => a._locs == b._locs;
        public static bool operator !=(LootFormation a, LootFormation b) => a._locs != b._locs;
    }


    [Serializable]
    public struct LootLocation {

        public static Vector2 DefaultSpeed => new Vector2(0f, -5f);

        public static LootLocation Zero {
            get {
                return new LootLocation(0f, 0f);
            }
        }

        public float x;
        public float y;
        public Vector2 speed;
        public float scale;
        /// <summary>
        /// The prefab associated with this loot. Intended to line up with the Enemy's array of prefabs.
        /// An int is used instead of a GameObject reference so that this struct can use the stack.
        /// </summary>
        public int prefabToUse;

        public LootLocation(float x, float y, Vector2 speed, float scale = 1f, int prefabToUse = 0) {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.scale = scale;
            this.prefabToUse = prefabToUse;
        }

        public override string ToString() {
            return $"Position: ({x},{y}), Speed: ({speed.x},{speed.y}), Scale: {scale}, Prefab: {prefabToUse}";
        }

        public LootLocation(LootLocation other) : this(other.x, other.y, other.speed, other.scale, other.prefabToUse) { }
        public LootLocation(float x, float y) : this(x, y, DefaultSpeed) { }
        public LootLocation(Vector2 position, Vector2 speed, float scale = 1f) : this(position.x, position.y, speed, scale) { }
        public LootLocation(Vector2 position) : this(position.x, position.y) { }
    }

}