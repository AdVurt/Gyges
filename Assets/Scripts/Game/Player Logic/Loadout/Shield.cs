using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Shield", fileName = "shield")]
    public class Shield : ShipPart {

        public override PartType ShipPartType {
            get {
                return PartType.Shield;
            }
        }

        [Tooltip("Maximum shield amount")]
        [Range(0f,100f)]
        public float shieldMax = 40f;

        [Tooltip("Shield percentage regenerated per second.")]
        public float regenRate = 1f;

        [Tooltip("Amount of power required to regenerate for one second.")]
        public float costPerSecond = 1f;

        [Tooltip("The minimum amount of power that is needed in order for regeneration to activate on this shield.")]
        [Range(0f, 100f)]
        public float powerThreshold = 50f;

        [Tooltip("The amount of time (in seconds) after taking damage before this shield can regenerate.")]
        public float regenDelay = 1f;

    }

}