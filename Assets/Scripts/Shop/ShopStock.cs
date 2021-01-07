using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [System.Serializable]
    public class ShopStock {

#if UNITY_EDITOR
        public bool foldout = false;
#endif

        public const int maximumItemListSize = 10;

        public Hull[] hulls;
        public FrontWeapon[] frontWeapons;
        public RearWeapon[] rearWeapons;
        public Shield[] shields;
        public Generator[] generators;
        public SpecialWeapon[] leftSpecials;
        public SpecialWeapon[] rightSpecials;

    }

}