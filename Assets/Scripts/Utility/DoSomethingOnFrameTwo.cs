using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Utility {

    public class DoSomethingOnFrameTwo : MonoBehaviour {
        public UnityEvent frameTwoEvent;
        private bool _frameOne = true;

        void OnEnable() {
            _frameOne = true;
        }

        // Update is called once per frame
        void Update() {
            if (_frameOne)
                _frameOne = false;
            else {
                frameTwoEvent.Invoke();
                enabled = false;
            }
        }
    }

}