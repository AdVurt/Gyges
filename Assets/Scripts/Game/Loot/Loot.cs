using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gyges.Utility;

namespace Gyges.Game {

    public class Loot : MonoBehaviour {

        [SerializeField] private GameObject _lootText = default;
        [SerializeField] private AudioClip _lootSound = default;

        private Transform _transform;
        [Header("Behaviour")]
        public Vector2 velocity;

        [Header("Loot Info")]
        public int monetaryAmount = 0;

        void Awake() {
            _transform = GetComponent<Transform>();
        }


        public void OnTriggerEnter2D(Collider2D collider) {
            if (collider.gameObject.tag == "Player") {
                if (monetaryAmount > 0) {
                    Player player = collider.GetComponent<Player>();
                    player.AddFunds(monetaryAmount);
                    if (_lootSound != null)
                        player.PlayLootSound(_lootSound);

                    Instantiate(_lootText,_transform.position + Vector3.back * 3f,Quaternion.identity).GetComponent<LootText>().SetLootAmount(monetaryAmount);
                }

                Destroy(gameObject);
            }
        }

        void Update() {
            _transform.position += (Vector3)(velocity * Time.deltaTime);

            if (velocity == Vector2.zero ||
                (velocity.x > 0f && _transform.position.x > Projectile.borders.xMax) ||
                (velocity.x < 0f && _transform.position.x < Projectile.borders.xMin) ||
                (velocity.y > 0f && _transform.position.y > Projectile.borders.yMax) ||
                (velocity.y < 0f && _transform.position.y < Projectile.borders.yMin)
                ) {
                Destroy(gameObject);
            }
        }

    }

}