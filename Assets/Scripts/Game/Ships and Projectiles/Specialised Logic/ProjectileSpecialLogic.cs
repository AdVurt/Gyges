using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public abstract class ProjectileSpecialLogic : MonoBehaviour {

        [System.NonSerialized] public Projectile projectile;

        public abstract void LogicOnEnable();
        public abstract void LogicUpdate();
    }

}