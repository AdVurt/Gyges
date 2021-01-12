using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gyges.Game {

    public class LootText : MonoBehaviour {

        public float lifetime = 0.5f;
        private float _timer = 0f;
        private TextMeshPro _text;

        void Awake() {
            _text = GetComponent<TextMeshPro>();
        }

        public int GetLootAmount() {
            return int.Parse(_text.text ?? "0");
        }

        public void SetLootAmount(int amount) {
            _text.text = amount.ToString();
        }

        // Update is called once per frame
        void Update() {
            if (!Global.enableGameLogic)
                return;

            _timer += Time.deltaTime;
            if (_timer >= lifetime)
                Destroy(gameObject);
        }
    }

}