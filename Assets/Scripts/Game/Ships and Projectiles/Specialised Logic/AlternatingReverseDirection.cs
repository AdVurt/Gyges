using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    /// <summary>
    /// If the launcher that launched this projectile has launched an odd number of rounds before this one, invert the direction of this projectile.
    /// </summary>
    public class AlternatingReverseDirection : ProjectileSpecialLogic {

        public enum HorizVert {
            Horizontal,
            Vertical
        }

        public HorizVert flipPositionAcross;

        public override void LogicOnEnable() {

            if ((projectile.launcherRoundsFired % 2) == 1) {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f , 180f));

                Vector3 newOffset = transform.position;
                if (flipPositionAcross == HorizVert.Horizontal) {
                    newOffset.x -= projectile.offset.x;
                }
                else {
                    newOffset.y -= projectile.offset.y;
                }

                transform.position = newOffset;
            }
        }

        public override void LogicUpdate() {
            
        }
    }

}