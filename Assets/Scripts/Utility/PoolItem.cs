using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Pooling {

    /// <summary>
    /// Inherit from this class if you wish to use item pooling.
    /// Instead of calling Destroy, to enable pooling, call ReturnToPool.
    /// Do not set default values (health for example) in Start or Awake, do this in OnEnable instead.
    /// </summary>
    public abstract class PoolItem : MonoBehaviour {

        internal event Action<GameObject> onReturn;

        /// <summary>
        /// Disables this game object and returns it to the object pool it game from.
        /// If it was not created by an object pool, the game object is instead destroyed.
        /// </summary>
        public void ReturnToPool() {
            if (onReturn == null) {
                Destroy(gameObject);
            }
            else {
                gameObject.SetActive(false);
                onReturn.Invoke(gameObject);
            }
        }
    }

}