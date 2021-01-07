using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [CreateAssetMenu(menuName = "Gyges/Ship Part/Hull", fileName = "hull")]
    public class Hull : ShipPart {

        public override PartType ShipPartType {
            get {
                return PartType.Hull;
            }
        }

        [Tooltip("The amount that this hull starts with.")]
        [Range(1f,100f)]
        public float startingHull = 40f;

        public Mesh shipModel = null;
        public Material[] modelMaterials = new Material[0];
        public Vector3[] trailPositions = new Vector3[0];
    }

}