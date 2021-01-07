using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {
    public abstract class GameObj : MonoBehaviour {
        
        public event Action<GameObj> OnDeath;

        protected void Die() {
            OnDeath?.Invoke(this);
        }

        public static void Kill(GameObj target) => target.Die();

    }
}