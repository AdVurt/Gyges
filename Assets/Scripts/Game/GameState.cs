using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(fileName = "Global State", menuName = "Gyges/Game State")]
    public class GameState : ScriptableObject {

        public Level currentLevel;
        public Level[] availableLevels;
        public Loadout[] loadouts;
        public int totalPoints = 0;
        public ShopStock shopStock;

        public void SetState(Level currentLevel, Level[] availableLevels, int totalPoints, ShopStockAsset stock, params Loadout[] loadouts) {
            this.currentLevel = currentLevel;
            this.availableLevels = availableLevels;
            this.loadouts = loadouts;
            this.totalPoints = totalPoints;
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
            shopStock = other.shopStock;
        }
    }

}