using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gyges.Game {
    public class PauseTrigger : MonoBehaviour {

        public UnityEvent m_OnPaused;
        public UnityEvent m_OnResumed;

        void OnEnable() {
            Global.OnPausedChanged += OnPausedChanged;
        }

        void OnDisable() {
            Global.OnPausedChanged -= OnPausedChanged;
        }

        void OnPausedChanged(bool val) {
            if (val)
                m_OnPaused.Invoke();
            else
                m_OnResumed.Invoke();
        }
    }
}