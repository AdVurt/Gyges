using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gyges.Game {
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : Pooling.PoolItem {

        [HideInInspector] public Vector2 velocity = Vector2.zero;
        [HideInInspector] public float scale = 1f;
        private Vector3 _baseScale = Vector3.zero;

        public static readonly Rect borders = new Rect(-20f,-10f,40f,20f);
        [HideInInspector] public float damage = 0f;
        [HideInInspector] public uint launcherRoundsFired = 0;
        [HideInInspector] public Vector2 offset = Vector2.zero; //What offset was this projectile given? Sometimes relevant.

        //Piercing logic
        [Tooltip("Does this projectile pierce through enemies?")]
        public bool piercing = false;
        [ConditionalHide("piercing"), Tooltip("How many enemies can this hit? 0 = infinite.")]
        public int hitsLifetime = 0;
        private HashSet<CollidesWithProjectiles> _hits = new HashSet<CollidesWithProjectiles>();

        //Special behaviour logic
        public ProjectileSpecialLogic[] specialLogic;
        

        private void OnEnable() {

            if (_baseScale == Vector3.zero) {
                _baseScale = transform.localScale;
            }
            transform.localScale = _baseScale * scale;
            _hits.Clear();

            ProjectileManager.Instance.OnUpdate += ProjectileUpdate;
            foreach (ProjectileSpecialLogic log in specialLogic) {
                if (log != null) {
                    log.projectile = this;
                    log.LogicOnEnable();
                }
            }
        }

        private void OnDisable() {
            if (ProjectileManager.Instance != null)
                ProjectileManager.Instance.OnUpdate -= ProjectileUpdate;
        }

        /// <summary>
        /// This acts as a replacement for the Update function.
        /// It is called from ProjectileManager's Update function. As a result, it should be more performant.
        /// </summary>
        public void ProjectileUpdate() {

            foreach (ProjectileSpecialLogic log in specialLogic) {
                if (log != null) {
                    log.LogicUpdate();
                }
            }

            transform.Translate(velocity * Time.deltaTime, Space.Self);
            Vector3 trUp = transform.localToWorldMatrix * velocity;

            if (velocity == Vector2.zero ||
                (trUp.x > 0f && transform.position.x > borders.xMax) ||
                (trUp.x < 0f && transform.position.x < borders.xMin) ||
                (trUp.y > 0f && transform.position.y > borders.yMax) ||
                (trUp.y < 0f && transform.position.y < borders.yMin)
                ) {
                ReturnToPool();
            }
        }

        /// <summary>
        /// This is called by a target that this projectile hits.
        /// Damage calculation should have been done by the target.
        /// </summary>
        /// <param name="source">The object that was hit by this projectile. Currently has no effect.</param>
        public void Hit(CollidesWithProjectiles source) {
            _hits.Add(source);

            if (!piercing || (hitsLifetime > 0 && _hits.Count >= hitsLifetime ) )
                ReturnToPool();
        }

        /// <summary>
        /// Returns whether or not this projectile can collide with the given object.
        /// Used in the case that this projectile is piercing and has already hit the given object.
        /// </summary>
        public bool CanCollideWith(CollidesWithProjectiles obj) {
            return !_hits.Contains(obj);
        }
    }
}