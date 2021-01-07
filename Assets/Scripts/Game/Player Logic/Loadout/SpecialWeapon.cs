using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Special", fileName = "special")]
    public class SpecialWeapon : AmmoWeapon {

        public override PartType ShipPartType {
            get {
                return PartType.Special;
            }
        }

    }

}