using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {

    [CreateAssetMenu(fileName = "Global State", menuName = "Gyges/Game State")]
    public class GameState : ScriptableObject {

        public Level currentLevel;
        public Level[] availableLevels;
        public Loadout[] loadouts;
        public int totalPoints = 0;

        [NonSerialized] private int _pendingPoints = 0;
        /// <summary>
        /// How many points are pending for the end of the current level?
        /// </summary>
        public int PendingPoints {
            get {
                return _pendingPoints;
            }
            set {
                bool invoke = _pendingPoints != value;
                _pendingPoints = value;
                if (invoke)
                    onPendingPointsChanged?.Invoke();
            }
        }

        public ShopStock shopStock;


        public event Action onPendingPointsChanged;

        public void SetState(Level currentLevel, Level[] availableLevels, int totalPoints, int pendingPoints, ShopStockAsset stock, params Loadout[] loadouts) {
            this.currentLevel = currentLevel;
            this.availableLevels = availableLevels;
            this.loadouts = loadouts;
            this.totalPoints = totalPoints;
            PendingPoints = pendingPoints;
            shopStock = (ShopStock)stock.stock.Clone();
        }

        /// <summary>
        /// Copies the state of another provided game state.
        /// </summary>
        public void CopyFrom(GameState other) {
            currentLevel = other.currentLevel;
            availableLevels = other.availableLevels;
            loadouts = other.loadouts;
            totalPoints = other.totalPoints;
            PendingPoints = other._pendingPoints;
            shopStock = other.shopStock;
        }
    }

}