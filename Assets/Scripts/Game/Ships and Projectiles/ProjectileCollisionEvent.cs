using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Game {
    [System.Serializable]
    public class ProjectileCollisionEvent : UnityEvent<ProjectileCollision> {

    }

    /// <summary>
    /// Stores the data to be passed by a projectile collision event
    /// </summary>
    public struct ProjectileCollision {

        public float damage;
        public Vector2 position;

        public ProjectileCollision(Collision2D physicsCollision, Projectile projectile) {
            damage = projectile.damage;
            position = physicsCollision.GetContact(0).point;
        }
    }
}