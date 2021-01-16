using UnityEngine;
using System;

namespace Gyges.Utility {

    public class EventOnDestroy : MonoBehaviour {

        public event Action onDestroy;

        void OnDestroy() {
            onDestroy?.Invoke();
        }
    }

}