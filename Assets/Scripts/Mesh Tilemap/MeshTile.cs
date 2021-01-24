using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.MeshTilemapBuilder {

    [Serializable]
    public class MeshTile {
        public enum TileMode {
            Dynamic,
            Override
        }

        public TileMode mode = TileMode.Dynamic;
        public Mesh overrideMesh = null;
        public Material[] overrideMaterials = new Material[0];
    }

}