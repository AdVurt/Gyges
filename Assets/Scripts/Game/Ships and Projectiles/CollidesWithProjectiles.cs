using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges;

namespace Gyges.Game {
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CollidesWithProjectiles : MonoBehaviour {

        public ProjectileCollisionEvent OnBulletHit;
        private Collider2D _collider;

        private void Awake() {
            _collider = GetComponent<Collider2D>();
        }

        void OnCollisionEnter2D(Collision2D collision) {

            if (collision.gameObject.TryGetComponent(out Projectile prj)) {

                if (prj.CanCollideWith(this)) {

                    OnBulletHit?.Invoke(new ProjectileCollision(collision, prj));
                    prj.Hit(this);

                }
            }

        }
    }
}