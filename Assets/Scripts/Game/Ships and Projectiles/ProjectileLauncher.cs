using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gyges.Pooling;

namespace Gyges.Game {
    public class ProjectileLauncher : MonoBehaviour {

        public Vector2 originOffset;
        public uint roundsFired = 0;

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

            foreach (ProjectileLocation loc in formation) {

                Vector3 pos = transform.position + (Vector3)(originOffset + new Vector2(loc.x, loc.y));
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

                ObjectPoolManager.GetPool(prefabs[loc.prefabToUse]).Spawn(pos, rot, preSpawnBehaviour);
                
            }
            roundsFired++;
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(transform.position + (Vector3)originOffset,0.5f);
        }
#endif
    }
}
