using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gyges.Pooling;
using Gyges.Utility;

namespace Gyges.Game {
    public class ProjectileLauncher : MonoBehaviour {

        public Vector2 originOffset;
        public uint roundsFired = 0;
        public float oscillationSpeed = 0f;
        public Vector2 oscillationMinValue = Vector2.left * 0.5f;
        public Vector2 oscillationMaxValue = Vector2.right * 0.5f;

        public float oscillatedAmount = 0f;

        private static Transform _projectileParent = null;
        public static Transform ProjectileParent {
            get {
                if (_projectileParent == null) {
                    _projectileParent = new GameObject("--- Player Projectiles",typeof(EventOnDestroy)).transform;
                    _projectileParent.GetComponent<EventOnDestroy>().onDestroy += ClearProjectileParent;
                }
                return _projectileParent;
            }
        }

        public static void ClearProjectileParent() {
            _projectileParent = null;
        }

        /// <summary>
        /// Fires the provided projectile formation using a single projectile prefab.
        /// </summary>
        /// <param name="prefab">The projectile prefab to use.</param>
        /// <param name="formation">The formation to use.</param>
        /// <param name="damage">How much damage each projectile does.</param>
        public void Fire(GameObject prefab, ProjectileFormation formation, float damage) {
            Fire(new GameObject[] { prefab }, formation, damage);
        }

        /// <summary>
        /// Fires the provided projectile formation using an array of projectile prefabs.
        /// </summary>
        /// <param name="prefabs">The projectile prefabs to use.</param>
        /// <param name="formation">The formation to use.</param>
        /// <param name="damage">How much damage each projectile does.</param>
        public void Fire(GameObject[] prefabs, ProjectileFormation formation, float damage) {

            float oscilLerpValue = 0.5f;
            if (oscillationSpeed > 0f) {
                oscilLerpValue = (oscillatedAmount >= 1f) ? (2f - oscillatedAmount) : oscillatedAmount;
            }

            foreach (ProjectileLocation loc in formation) {

                Vector3 pos = transform.position + (Vector3)(originOffset + new Vector2(loc.x, loc.y)) + (Vector3)Vector2.Lerp(oscillationMinValue, oscillationMaxValue, oscilLerpValue);
                Quaternion rot = Quaternion.Euler(0f, 0f, -loc.rotation);

                Action<GameObject> preSpawnBehaviour =
                    (o) => {
                        Projectile proj = o.GetComponent<Projectile>();
                        proj.velocity = loc.speed;
                        proj.damage = damage * loc.damageMultiplier;
                        proj.scale = loc.scale;
                        proj.launcherRoundsFired = roundsFired;
                        proj.offset = originOffset + new Vector2(loc.x, loc.y);
                    };

                ObjectPoolManager.GetPool(prefabs[loc.prefabToUse]).Spawn(pos, rot, preSpawnBehaviour, ProjectileParent);
                
            }
            roundsFired++;
        }

        void Update() {
            if (oscillationSpeed > 0f) {
                oscillatedAmount += oscillationSpeed * Time.deltaTime;

                if (oscillatedAmount > 2f)
                    oscillatedAmount -= 2f;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(transform.position + (Vector3)originOffset,0.5f);
        }
#endif
    }
}
