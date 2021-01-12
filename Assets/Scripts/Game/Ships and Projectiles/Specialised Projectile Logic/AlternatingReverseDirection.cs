using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class AlternatingReverseDirection : ProjectileSpecialLogic {

        public bool flipHorizontal = false;
        public bool flipVertical = false;
        public uint divisor = 2;
        public uint remainder = 1;

        public override void LogicOnEnable() {

            if ((projectile.launcherRoundsFired % divisor) == remainder) {

                Vector3 newOffset = transform.position;
                Vector3 direction = transform.rotation * Vector3.up;

                if (flipHorizontal) {
                    newOffset.x -= projectile.offset.x;
                    direction.x = -direction.x;
                }
                if (flipVertical) {
                    newOffset.y -= projectile.offset.y;
                    direction.y = -direction.y;
                }

                transform.position = newOffset;
                transform.rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
            }

            /*
            if ((projectile.launcherRoundsFired % 2) == 1) {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f , 180f));
            }
            */
        }

        public override void LogicUpdate() {
            
        }
    }

}