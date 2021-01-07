using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Gyges.Game {
    [CreateAssetMenu(menuName = "Gyges/Ship Part/Front Weapon", fileName = "frontWeapon")]
    public class FrontWeapon : LevelableWeapon {

        public override bool HasAlternateFireMode {
            get {
                return false;
            }
        }

        public override PartType ShipPartType {
            get {
                return PartType.FrontWeapon;
            }
        }

    }
}