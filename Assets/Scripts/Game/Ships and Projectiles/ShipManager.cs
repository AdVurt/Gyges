using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {
    public class ShipManager : MonoBehaviour {

        public static ShipManager Instance { get; private set; }

        private HashSet<Ship> _allShips = new HashSet<Ship>();
        private HashSet<Enemy> _allEnemies = new HashSet<Enemy>();
        private HashSet<Player> _allPlayers = new HashSet<Player>();
        
        public void AddShip(Ship ship) {
            if (_allShips.Add(ship)) {
                if ((ship.teams & Ship.Teams.Player) > 0) {
                    _allPlayers.Add(ship.GetComponent<Player>());
                }
                if ((ship.teams & Ship.Teams.Enemy) > 0) {
                    _allEnemies.Add(ship.GetComponent<Enemy>());
                }
            }
        }

        public void RemoveShip(Ship ship) {
            if (_allShips.Remove(ship)) {
                if ((ship.teams & Ship.Teams.Player) > 0) {
                    _allPlayers.Remove(ship.GetComponent<Player>());
                }
                if ((ship.teams & Ship.Teams.Enemy) > 0) {
                    _allEnemies.Remove(ship.GetComponent<Enemy>());
                }
            }
        }


        void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (Instance == this)
                Instance = null;
        }


    }
}