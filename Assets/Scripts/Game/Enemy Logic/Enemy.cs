using System.Collections;
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

        private Collider2D _collider;
        private Ship _ship;
        private Material _material;
        private static readonly int _dissolveAmountMatFloat = Shader.PropertyToID("_DissolveAmount");

        [Header("Game Data")]
        public FloatReference startingHealth;
        public IntReference bounty;
        

        private float _dissolveAmount = 0f;
        [Header("Visual")]
        [SerializeField] [Range(1f,10f)] private float _dissolveSpeed = 4f;

        public event Action<IWaveObjectDestroyEventParams> OnDestroy;

        void Awake() {
            _ship = GetComponent<Ship>();
            _ship.Health = startingHealth.Value;
            _collider = GetComponent<Collider2D>();
            _material = GetComponent<Renderer>().material;
            _ship.OnDeath += _ship_OnDeath;
        }

        private void _ship_OnDeath(GameObj obj) {
            _collider.enabled = false;
            StartCoroutine(DissolveOut());
        }

        public void TakeDamageFromCollision(ProjectileCollision coll) {
            _ship.Health -= coll.damage;
        }

        /// <summary>
        /// Instantly destroys this game object, calling the OnDestroy event in the process.
        /// </summary>
        /// <param name="killedByPlayer">Was it killed by the player? (For example, should points be awarded?)</param>
        public void Kill(bool killedByPlayer = true) {
            OnDestroy?.Invoke(new IWaveObjectDestroyEventParams() { waveObject = this, killedByPlayer = killedByPlayer });
            Destroy(gameObject);
        }

        private IEnumerator DissolveOut(bool killedByPlayer = true) {
            while (_dissolveAmount < 1f) {
                _dissolveAmount += _dissolveSpeed * Time.deltaTime;
                _material.SetFloat(_dissolveAmountMatFloat, _dissolveAmount);
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

            Undo.RegisterCreatedObjectUndo(go, "Create Enemy");
            Selection.activeObject = go;
        }

#endif

    }

}