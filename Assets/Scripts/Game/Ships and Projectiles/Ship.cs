using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {
    public class Ship : GameObj {

        private bool _addedToManager = false;

        [Flags]
        public enum Teams {
            None = 0,
            Player = 1,
            Enemy = 2
        }

        private float _health;
        public float Health {
            get { return _health; }
            set {
                _health = value;
                OnHealthChanged?.Invoke(_health);
                if (_health <= 0f) { Die(); }
            }
        }
        public Teams teams = Teams.None;

        public event Action<float> OnHealthChanged;

        void OnEnable() {
            if (ShipManager.Instance != null) {
                ShipManager.Instance.AddShip(this);
                _addedToManager = true;
            }
        }

        void OnDisable() {
            if (ShipManager.Instance != null && _addedToManager) {
                ShipManager.Instance.RemoveShip(this);
            }
        }

        void Start() {
            if (!_addedToManager) {
                ShipManager.Instance.AddShip(this);
                _addedToManager = true;
            }
        }
    }
}