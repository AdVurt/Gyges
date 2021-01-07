using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gyges.Pooling {
    public static class ObjectPoolManager {

        private static Dictionary<GameObject, ObjectPool> _pools = new Dictionary<GameObject, ObjectPool>();
        /// <summary>
        /// Is the manager listening out for scene changes?
        /// </summary>
        private static bool _listeningForSceneChanges = false;

        /// <summary>
        /// Creates a pool of objects for the specified prefab and returns it. If the pool already exists, simply returns it.
        /// </summary>
        /// <param name="obj">The prefab to create a pool for</param>
        /// <param name="initialPoolSize">The desired starting size of the object pool</param>
        /// <returns>The newly created pool (or the already existing one, if it exists already)</returns>
        public static ObjectPool PoolSetup(GameObject obj, int initialPoolSize) {

            if (!_listeningForSceneChanges) {
                SceneManager.activeSceneChanged += ChangedScene;
                _listeningForSceneChanges = true;
            }

            if (_pools.ContainsKey(obj)) {
                return _pools[obj];
            }
            else {
                ObjectPool newPool = new ObjectPool(obj, initialPoolSize);
                _pools.Add(obj, newPool);
                return newPool;
            }
        }

        /// <summary>
        /// Returns a pool of objects for the specified prefab.
        /// </summary>
        public static ObjectPool GetPool(GameObject obj) {
            if (!PoolExists(obj))
                PoolSetup(obj, 1);
            return _pools[obj];
        }

        public static bool PoolExists(GameObject obj) => _pools.ContainsKey(obj);

        /// <summary>
        /// Clears a given pool. Don't call this if any instances spawned from it are still active.
        /// </summary>
        public static void ClearPool(GameObject obj) {
            if (PoolExists(obj))
                _pools[obj].Clear();
        }

        /// <summary>
        /// Clears all pools. Don't call this if any instances spawned from them are still active.
        /// </summary>
        public static void ClearAllPools() {
            foreach(ObjectPool pool in _pools.Values) {
                pool.Clear();
            }
            _pools.Clear();
        }

        /// <summary>
        /// The scene has been changed - clear all pools.
        /// </summary>
        private static void ChangedScene(Scene current, Scene next) {
            ClearAllPools();
        }
    }
}