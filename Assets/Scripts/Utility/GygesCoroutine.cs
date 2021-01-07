using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Utility {
    public static class GygesCoroutine {
        /// <summary>
        /// Runs a coroutine as a synchronous method.
        /// </summary>
        public static void RunCoroutineSynchronous(IEnumerator coroutine) {
            while (coroutine.MoveNext()) {
                if (coroutine.Current == null)
                    return;
                RunCoroutineSynchronous((IEnumerator)coroutine.Current);
            }
        }

    }
}