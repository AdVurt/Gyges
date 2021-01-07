using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Generator", fileName = "generator")]
    public class Generator : ShipPart {

        public override PartType ShipPartType {
            get {
                return PartType.Generator;
            }
        }

        [Tooltip("Amount of power generated per second")]
        public float powerGeneration = 5f;

    }
}