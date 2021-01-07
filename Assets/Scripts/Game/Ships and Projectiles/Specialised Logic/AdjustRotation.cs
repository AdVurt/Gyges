using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class AdjustRotation : ProjectileSpecialLogic {

        private Transform _transform;

        public Vector3 desiredRotation = Vector3.zero;
        public float rotateSpeed = 3f;

        public override void LogicOnEnable() {
            _transform = GetComponent<Transform>();
        }

        public override void LogicUpdate() {
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * rotateSpeed * 360f);
        }

    }

}