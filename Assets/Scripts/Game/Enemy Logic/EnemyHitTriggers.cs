using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {
    [RequireComponent(typeof(Enemy))]
    public class EnemyHitTriggers : MonoBehaviour {

        private Enemy _enemy;
        private Renderer[] _renderers;
        private readonly int _flashMatFloat = Shader.PropertyToID("_Flash");
        
        [Header("Flash Effect")]
        [SerializeField]
        private float _flashDuration = 0.1f;
        private float _flashTimer = 0f;
        private bool _flashCoRtRunning = false;

        void Awake() {
            _enemy = GetComponent<Enemy>();
            _renderers = new Renderer[_enemy.AdditionalRenderers.Length+1];
            _renderers[0] = GetComponent<Renderer>();
            for (int i = 0; i < _enemy.AdditionalRenderers.Length; i++) {
                _renderers[i + 1] = _enemy.AdditionalRenderers[i];
            }
        }


        //Functions in this class need to return void and have a single ProjectileCollision parameter.
        //This is so that they can be called from CollidesWithProjectiles.

        /// <summary>
        /// Use this enemy's main sprite material flash data.
        /// </summary>
        /// <param name="proj"></param>
        public void FlashColour(ProjectileCollision proj) {
            _flashTimer = _flashDuration;
            if (!_flashCoRtRunning) {
                StartCoroutine(Flash());
                _flashCoRtRunning = true;
            }
        }

        private IEnumerator Flash() {

            foreach (Renderer ren in _renderers) {
                foreach (Material mat in ren.materials) {
                    mat.SetFloat(_flashMatFloat, 1f);
                }
            }
            
            while (_flashTimer > 0f) {
                _flashTimer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            foreach (Renderer ren in _renderers) {
                foreach (Material mat in ren.materials) {
                    mat.SetFloat(_flashMatFloat, 0f);
                }
            }

            _flashCoRtRunning = false;
        }


    }
}