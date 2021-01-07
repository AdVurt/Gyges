using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gyges.Game {
    public class TimeBasedEnemyActions : EnemyActions {


        protected override void OnEnable() {
            StartCoroutine(PlayAllCoRtsInSequence());
        }

        private IEnumerator PlayAllCoRtsInSequence() {

            while (_actionQueue.Count > 0) {
                yield return StartCoroutine(ExecuteCoRt(_actionQueue.Dequeue()));
            }
            Finished();
            yield return null;
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected() {
            
        }
#endif
    }
}