using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    /// <summary>
    /// Calls a single enemy action. Then, when the enemy is off screen, self-destructs.
    /// </summary>
    public class SingleEnemyAction : EnemyActions {

        protected override void OnEnable() {
            //Run the assigned coroutine as a synchronous method.
            Utility.GygesCoroutine.RunCoroutineSynchronous(ExecuteCoRt(_actionQueue.Dequeue()));
        }

        protected override void OnDrawGizmosSelected() {

            QueuedEnemyAction action = _actions[0];
            switch (action.actionType) {
                case QueuedEnemyAction.ActionType.SetVelocity:
                    Gizmos.DrawRay(transform.position, action.vector2Values[0].normalized * 10f);
                    break;
                case QueuedEnemyAction.ActionType.SetRotationSpeed: //These don't work with a single action
                case QueuedEnemyAction.ActionType.Loop:
                case QueuedEnemyAction.ActionType.WaitForSeconds:
                    break;
                default:
                    throw new System.Exception("Unknown action type");
            }

        }
    }

}