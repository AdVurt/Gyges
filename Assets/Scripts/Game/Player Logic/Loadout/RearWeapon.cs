using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Rear Weapon", fileName = "rearWeapon")]
    public class RearWeapon : LevelableWeapon {

        public WeaponLogic[] altLevelStats = new WeaponLogic[11];

        [SerializeField] private bool _hasAlternateFireMode = default;
        public override bool HasAlternateFireMode {
            get {
                return _hasAlternateFireMode;
            }
        }

        public override PartType ShipPartType {
            get {
                return PartType.RearWeapon;
            }
        }

    }

}