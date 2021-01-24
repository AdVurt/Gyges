using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gyges.Game {

    [RequireComponent(typeof(Enemy))]
    public class EnemyDeathModel : MonoBehaviour {

        private Enemy _enemy;
        [Tooltip("Requires a gameobject that is a child of this - it will be swapped in and activated, so it will appear to be a change of state for this object."), SerializeField]
        private GameObject _newModel = default;

        void Awake() {
            _enemy = GetComponent<Enemy>();
            _enemy.onDeathAnimationStarted += EnemyKilled;
        }

        private void EnemyKilled() {
            _newModel.SetActive(true);
            _newModel.transform.SetParent(transform.parent);
        }
    }

}
