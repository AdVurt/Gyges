using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Pooling {

    /// <summary>
    /// Holds a reference to a game object (prefab), and handles the creation and pooling of it
    /// if that object has a PoolItem component attached to it.
    /// If it doesn't have a PoolItem attached, there will be errors, as this is designed for maximum
    /// performance and checking the component will cause a small overhead.
    /// </summary>
    public class ObjectPool {

        private Stack<GameObject> _inactiveObjects;
        private GameObject _prefab;

        internal ObjectPool(GameObject prefab, int initialPoolSize) {
            _prefab = prefab;
            _inactiveObjects = new Stack<GameObject>(initialPoolSize);
            AddToPool(initialPoolSize);

        }

        /// <summary>
        /// Adds a game object to the pool.
        /// This object should already be in a disabled state, and it should have already been created by this pool.
        /// </summary>
        private void Pool(GameObject obj) {
            _inactiveObjects.Push(obj);
        }

        /// <summary>
        /// Retrieves a game object from the pool, spawns it at the specified position and rotation, and returns it.
        /// </summary>
        /// <returns>The spawned object.</returns>
        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null) => Spawn(position, rotation, null, parent);

        /// <summary>
        /// Retrieves a game object from the pool, spawns it at the specified position and rotation, takes some action before activating it, then returns it.
        /// </summary>
        /// <param name="position">Where to spawn the object.</param>
        /// <param name="rotation">The object's spawning rotation.</param>
        /// <param name="preEnableAction">Any actions that should be done to the object before activating it.</param>
        /// <returns>The spawned object.</returns>
        public GameObject Spawn(Vector3 position, Quaternion rotation, Action<GameObject> preEnableAction, Transform parent = null) {
            GameObject obj = Retrieve();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            preEnableAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Retrieves a game object from the pool, spawns it at the specified position (with default rotation), and returns it.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(Vector3 position) => Spawn(position, Quaternion.identity);

        /// <summary>
        /// Returns a game object from the pool (creating it if the pool is empty).
        /// This removes it from the _inactiveObjects stack.
        /// </summary>
        public GameObject Retrieve() {
            while (_inactiveObjects.Count > 0 && _inactiveObjects.Peek() == null) {
                _inactiveObjects.Pop();
            }
            if (_inactiveObjects.Count == 0) {
                AddToPool();
            }
            GameObject obj = _inactiveObjects.Pop();
            if (obj == null) {
                AddToPool();
                obj =_inactiveObjects.Pop();
            }
            return obj;
        }

        /// <summary>
        /// Adds a new copy of the prefab to the pool. The copy will be disabled by this process.
        /// The PoolItem attached to the prefab will be given a reference to this pool.
        /// </summary>
        private void AddToPool() {
            GameObject obj = UnityEngine.Object.Instantiate(_prefab);
            obj.SetActive(false);
            obj.GetComponent<PoolItem>().onReturn += Pool;
            _inactiveObjects.Push(obj);
        }

        /// <summary>
        /// Adds a number of new copies of the prefab to the pool.
        /// </summary>
        private void AddToPool(int count) {
            for (int i = 0; i < count; i++) {
                AddToPool();
            }
        }

        /// <summary>
        /// Clears the pool. Don't call this if any instances spawned from it are still active.
        /// </summary>
        public void Clear() {
            foreach (GameObject o in _inactiveObjects) {
                if (o != null) {
                    o.GetComponent<PoolItem>().onReturn -= Pool;
                    UnityEngine.Object.Destroy(o);
                }
            }
            _inactiveObjects.Clear();
        }

    }

}