using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Pooling;

namespace Gyges.Game {
    public class EnemyFirer : MonoBehaviour {
        public enum FireType {
            None,
            Constant,
            Burst
        }

        public enum ProjectileBehaviour {
            FlatDirection,
            AimedAtPlayer,
            Homing,
            RandomDirectionFromArray
        }
        
        [SerializeField] private FireType _fireType = FireType.None;
        [SerializeField] private GameObject _projectilePrefab = default;
        [SerializeField] private float _delayBetweenShots = 0.1f;
        [SerializeField] private ProjectileBehaviour _projectileBehaviour = ProjectileBehaviour.FlatDirection;
        [SerializeField] private Vector2 _direction = Vector2.down;
        [SerializeField] private Vector2 _speed = ProjectileLocation.DefaultSpeed;
        [SerializeField] private float _damage = 5f;
        [SerializeField] private bool _useFormation = false;
        [SerializeField] private ProjectileFormation _projectileFormation = ProjectileFormation.SingleStraight;
        [SerializeField] private Vector2[] _directionArray = new Vector2[] { Vector2.down };
        [SerializeField] private Vector2 _originOffset = default;

        private float _timer = 0f;
        private ObjectPool _pool;

        void Awake() {
            _pool = ObjectPoolManager.PoolSetup(_projectilePrefab, Mathf.CeilToInt(1f/_delayBetweenShots) );
        }

        void Update() {
            if (_fireType == FireType.None || !Global.enableGameLogic || Global.Paused) {
                return;
            }

            while (_timer <= 0f) {
                _timer += _delayBetweenShots;
                Fire();
            }

            _timer -= Time.deltaTime;
        }

        void Fire() {

            Vector3 basePosition = transform.position + (Vector3)_originOffset;
            float baseRotation = Vector2.Angle(Vector2.up, _direction);

            if (_useFormation) {
                foreach (ProjectileLocation loc in _projectileFormation) {
                    float rot;
                    switch (_projectileBehaviour) {
                        case ProjectileBehaviour.FlatDirection:
                            rot = baseRotation + loc.rotation;
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                    SpawnProjectile(basePosition + new Vector3(loc.x, loc.y, 0f), rot, loc.speed, _damage);
                }
            }
            else {
                SpawnProjectile(basePosition, baseRotation, _speed, _damage);
            }

        }

        void SpawnProjectile(Vector3 pos, float rotation, Vector2 speed, float damage) {

            _pool.Spawn(pos, Quaternion.Euler(0f, 0f, rotation), (o) => {
                Projectile proj = o.GetComponent<Projectile>();
                proj.velocity = speed;
                proj.damage = damage;
            });

        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(transform.position + (Vector3)_originOffset,0.5f);
        }
#endif
    }
}