﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gyges.Utility;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace Gyges.Game {
    [RequireComponent(typeof(Ship))]
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour, IWaveObject {

        [Flags]
        public enum OutOfBoundsDirections {
            None = 0,
            North = 1,
            East = 2,
            South = 4,
            West = 8
        };

        private Collider2D _collider;
        private Ship _ship;
        private Vector3 _meshExtents;
        private Material[] _materials;
        private static readonly int _dissolveAmountMatFloat = Shader.PropertyToID("_DissolveAmount");
        private bool _dead = false;

        [Header("Game Data")]
        public FloatReference startingHealth;
        public IntReference bounty;
        public GameObject[] lootPrefabs = new GameObject[0];
        public LootFormation lootFormation = new LootFormation();

        private float _dissolveAmount = 0f;
        [Header("Visual")]
        [SerializeField] [Range(1f,10f)] private float _dissolveSpeed = 4f;
        [SerializeField, Tooltip("Renderers that should be affected by this other than the one directly attached to this object (for example, child object renderers).")] private Renderer[] _additionalRenderers = new Renderer[0];

        #region Interface-required properties
        public Vector2 Velocity {
            get {
                Vector2 result = Vector2.zero;
                foreach(IVelocitySetter vel in GetComponents<IVelocitySetter>()) {
                    result += vel.GetVelocity();
                }
                return result;
            }
        }

        public bool StartLogicBeforeGameplay { get => false; }
        #endregion

        public Renderer[] AdditionalRenderers {
            get {
                return _additionalRenderers;
            }
        }

        public event Action onDeathAnimationStarted;
        public event Action<IWaveObjectDestroyEventParams> onDestroy;

        void Awake() {
            _ship = GetComponent<Ship>();
            _ship.Health = startingHealth.Value;
            _collider = GetComponent<Collider2D>();
            _materials = GetComponent<Renderer>().materials;
            _meshExtents = Vector3.Scale(GetComponent<MeshFilter>().sharedMesh.bounds.extents, transform.lossyScale);
            _ship.OnDeath += _ship_OnDeath;

            if (startingHealth.Value <= 0) {
                Debug.Log($"Enemy {name} started with 0 health.");
                Destroy(gameObject);
            }
        }

        private void _ship_OnDeath(GameObj obj) {
            _collider.enabled = false;
            onDeathAnimationStarted?.Invoke();
            StartCoroutine(DissolveOut());
            foreach (Enemy enemy in GetComponentsInChildren<Enemy>()) {
                if (enemy != this)
                    enemy._ship_OnDeath(obj);
            }
        }

        public void TakeDamageFromCollision(ProjectileCollision coll) {
            _ship.Health -= coll.damage;
        }


        #region Interface-required methods
        public bool IsOutOfBounds() {
            return IsOutOfBounds(out OutOfBoundsDirections dummy);
        }

        public bool IsOutOfBounds(out OutOfBoundsDirections oobDir) {
            OutOfBoundsDirections outres = OutOfBoundsDirections.None;

            if (transform.position.x - _meshExtents.x > EnemyActions.borders.xMax)
                outres |= OutOfBoundsDirections.East;
            else if (transform.position.x + _meshExtents.x < EnemyActions.borders.xMin)
                outres |= OutOfBoundsDirections.West;

            if (transform.position.y - _meshExtents.y > EnemyActions.borders.yMax)
                outres |= OutOfBoundsDirections.North;
            else if (transform.position.y + _meshExtents.y < EnemyActions.borders.yMin)
                outres |= OutOfBoundsDirections.South;

            oobDir = outres;
            return outres != OutOfBoundsDirections.None;
        }

        public Transform GetTransform() => transform;
        #endregion

        /// <summary>
        /// Instantly destroys this game object, calling the OnDestroy event in the process.
        /// Awards this enemy's bounty value as points, and spawns any loot this enemy has been assigned.
        /// </summary>
        /// <param name="killedByPlayer">Was it killed by the player? (For example, should points be awarded?)</param>
        public void Kill(bool killedByPlayer = true) {
            if (_dead)
                return;

            onDestroy?.Invoke(new IWaveObjectDestroyEventParams(this, killedByPlayer, bounty.Value));

            // Spawn any loot.
            if (killedByPlayer && lootPrefabs.Length > 0 && lootFormation.Count > 0) {

                for (int i = 0; i < lootFormation.Count; i++) {
                    LootLocation loc = lootFormation[i];
                    if (loc.prefabToUse >= lootPrefabs.Length)
                        throw new IndexOutOfRangeException($"Enemy {name}, location {i} has a prefab ID of {loc.prefabToUse}, but the maximum is {lootPrefabs.Length-1}.");
                    if (lootPrefabs[loc.prefabToUse] == null)
                        throw new NullReferenceException($"Enemy {name}, prefab {loc.prefabToUse} is null.");
                    Instantiate(lootPrefabs[loc.prefabToUse], transform.position + new Vector3(loc.x,loc.y), Quaternion.identity).GetComponent<Loot>().velocity = loc.speed;
                }

            }

            _dead = true;
            Destroy(gameObject);
        }

        private IEnumerator DissolveOut(bool killedByPlayer = true) {
            while (_dissolveAmount < 1f) {
                _dissolveAmount += _dissolveSpeed * Time.deltaTime;
                foreach(Material mat in _materials) {
                    mat.SetFloat(_dissolveAmountMatFloat, _dissolveAmount);
                }
                foreach (Renderer ren in _additionalRenderers) {
                   foreach(Material mat in ren.materials) {
                        mat.SetFloat(_dissolveAmountMatFloat, _dissolveAmount);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            Kill(killedByPlayer);
        }

#if UNITY_EDITOR

        [MenuItem("GameObject/Create Default Enemy", false, 0)]
        public static void CreateBlankEnemy(MenuCommand menuCommand) {

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "New Enemy";
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy Ships");
            DestroyImmediate(go.GetComponent<BoxCollider>());
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<Rigidbody2D>();
            Ship ship = go.AddComponent<Ship>();
            Enemy enemy = go.AddComponent<Enemy>();
            CollidesWithProjectiles collidesWithProjectiles = go.AddComponent<CollidesWithProjectiles>();
            EnemyHitTriggers enemyHitTriggers = go.AddComponent<EnemyHitTriggers>();
            Rigidbody2D rigidbody2D = go.GetComponent<Rigidbody2D>();
            
            go.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/EnemyShip");
            ship.teams = Ship.Teams.Enemy;
            collidesWithProjectiles.OnBulletHit = new ProjectileCollisionEvent();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody2D.gravityScale = 0f;
            UnityEventTools.AddPersistentListener(collidesWithProjectiles.OnBulletHit,enemyHitTriggers.FlashColour);
            UnityEventTools.AddPersistentListener(collidesWithProjectiles.OnBulletHit,enemy.TakeDamageFromCollision);

            if (Selection.activeTransform != null) {
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
            }

            Undo.RegisterCreatedObjectUndo(go, "Create Enemy");
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Create Default Background Enemy", false, 0)]
        public static void CreateBlankBackgroundEnemy(MenuCommand menuCommand) {

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "New Background Enemy";
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy Ground Ships");
            DestroyImmediate(go.GetComponent<BoxCollider>());
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<Rigidbody2D>();
            Ship ship = go.AddComponent<Ship>();
            Enemy enemy = go.AddComponent<Enemy>();
            CollidesWithProjectiles collidesWithProjectiles = go.AddComponent<CollidesWithProjectiles>();
            EnemyHitTriggers enemyHitTriggers = go.AddComponent<EnemyHitTriggers>();
            Rigidbody2D rigidbody2D = go.GetComponent<Rigidbody2D>();

            go.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/EnemyShip");
            ship.teams = Ship.Teams.Enemy;
            collidesWithProjectiles.OnBulletHit = new ProjectileCollisionEvent();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody2D.gravityScale = 0f;
            UnityEventTools.AddPersistentListener(collidesWithProjectiles.OnBulletHit, enemy.TakeDamageFromCollision);

            if (Selection.activeTransform != null) {
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
            }

            Undo.RegisterCreatedObjectUndo(go, "Create Enemy");
            Selection.activeObject = go;

        }
#endif

    }

}