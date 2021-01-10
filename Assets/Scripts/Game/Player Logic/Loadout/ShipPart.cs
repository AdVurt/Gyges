using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public abstract class ShipPart : ScriptableObject {

        public enum PartType {
            Hull,
            FrontWeapon,
            RearWeapon,
            Shield,
            Generator,
            Special
        };

        public abstract PartType ShipPartType { get; }
        [Tooltip("The preview image that shows in UI ")]
        public Texture2D partIcon;
        [Tooltip("The base monetary cost of this part (for levelable weapons, this is the level 1 cost).")]
        public int baseCost;
        [Tooltip("How this part will be represented in in-game UI.")]
        public string inGameUIName;

        /// <summary>
        /// Returns the monetary value of this part. The parameter does nothing except on levelable weapons.
        /// </summary>
        public virtual int GetCost(int level = 0) {
            return baseCost;
        }

    }

}