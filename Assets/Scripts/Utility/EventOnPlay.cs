using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Game {

    public class EventOnPlay : MonoBehaviour {

        public UnityEvent onPlay;

        void Start() {
            onPlay.Invoke();
        }
    }

}