using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    public class ConstantRotation : MonoBehaviour {

        public Transform transformToRotate;
        public Vector3 rotationAmounts = Vector3.zero;

        private Transform _transform;


        // Update is called once per frame
        void Update() {
            transformToRotate.Rotate(rotationAmounts * Time.deltaTime, Space.World);
        }
    }

}