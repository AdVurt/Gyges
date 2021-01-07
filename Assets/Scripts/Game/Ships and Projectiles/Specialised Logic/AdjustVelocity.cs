using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class AdjustVelocity : ProjectileSpecialLogic {

        public Vector2 desiredVelocity = new Vector2(0f, 10f);
        public float changeSpeed = 3f;

        public override void LogicOnEnable() {
            
        }

        public override void LogicUpdate() {
            projectile.velocity = Vector2.MoveTowards(projectile.velocity, desiredVelocity, changeSpeed * Time.deltaTime);
        }
    }

}