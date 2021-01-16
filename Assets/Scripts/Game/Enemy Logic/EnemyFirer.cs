using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Pooling;
using Gyges.Utility;

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

        private static Transform _projectileParent = null;
        public static Transform ProjectileParent {
            get {
                if (_projectileParent == null) {
                    _projectileParent = new GameObject("--- Enemy Projectiles", typeof(EventOnDestroy)).transform;
                    _projectileParent.GetComponent<EventOnDestroy>().onDestroy += ClearProjectileParent;
                }
                return _projectileParent;
            }
        }

        public static void ClearProjectileParent() {
            _projectileParent = null;
        }


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
            basePosition.z = -1f;
            float baseRotation = Vector2.Angle(Vector2.up, _direction);
            float rot = baseRotation;

            switch (_projectileBehaviour) {
                case ProjectileBehaviour.FlatDirection:
                    break;
                case ProjectileBehaviour.AimedAtPlayer:
                    Vector3 playerPosition = Player.GetRandomInstance().transform.position;
                    rot = Vector2.Angle(playerPosition - basePosition, Vector2.up) * (playerPosition.x > basePosition.x ? -1f : 1f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }


            if (_useFormation) {
                foreach (ProjectileLocation loc in _projectileFormation) {
                    SpawnProjectile(basePosition + new Vector3(loc.x, loc.y), rot + loc.rotation, loc.speed, _damage);
                }
            }
            else {
                SpawnProjectile(basePosition, rot, _speed, _damage);
            }

        }

        void SpawnProjectile(Vector3 pos, float rotation, Vector2 speed, float damage) {

            _pool.Spawn(pos, Quaternion.Euler(0f, 0f, rotation), (o) => {
                Projectile proj = o.GetComponent<Projectile>();
                proj.velocity = speed;
                proj.damage = damage;
            }, ProjectileParent);

        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(transform.position + (Vector3)_originOffset,0.5f);
        }
#endif
    }
}