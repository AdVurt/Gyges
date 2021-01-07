using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gyges.Game {

    public class DemoScreenManager : MonoBehaviour {
        [SerializeField] private RawImage _img = default;

        // Start is called before the first frame update
        void Start() {
            _img.color = Color.black;
        }

        // Update is called once per frame
        void Update() {

            if (_img.color.a > 0f) {
                _img.color = new Color(0f,0f,0f, _img.color.a - Time.deltaTime);
            }

        }
    }

}