using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class SyncSpeedWithBackground : MonoBehaviour, IVelocitySetter {

        public MaterialAutoScroll autoScroll;

        public Vector2 GetVelocity() => autoScroll.GetScrollPerSecond();

        // Update is called once per frame
        void Update() {
            if (!Global.Paused && Global.enableGameLogic)
                transform.position += (Vector3)GetVelocity() * Time.deltaTime;
        }
    }

}