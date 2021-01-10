using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Rear Weapon", fileName = "rearWeapon")]
    public class RearWeapon : LevelableWeapon {

        public WeaponLogic[] altLevelStats = new WeaponLogic[11];

        public bool HasEnoughPower(int level, float currentPower, bool altMode)
            => (altMode ? altLevelStats : levelStats)[level - 1].powerCost <= currentPower;

        public override bool HasAlternateFireMode {
            get {
                return true;
            }
        }

        public override PartType ShipPartType {
            get {
                return PartType.RearWeapon;
            }
        }

    }

}