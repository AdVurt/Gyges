using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {

    [System.Serializable]
    public class ShopStock : System.ICloneable {

        public const int maximumItemListSize = 10;

        public Hull[] hulls = new Hull[0];
        public FrontWeapon[] frontWeapons = new FrontWeapon[0];
        public RearWeapon[] rearWeapons = new RearWeapon[0];
        public Shield[] shields = new Shield[0];
        public Generator[] generators = new Generator[0];
        public SpecialWeapon[] leftSpecials = new SpecialWeapon[0];
        public SpecialWeapon[] rightSpecials = new SpecialWeapon[0];

        public ShopStock() { }
        public ShopStock(ShopStock other) {
            hulls = (Hull[])hulls.Clone();
            frontWeapons = (FrontWeapon[])frontWeapons.Clone();
            rearWeapons = (RearWeapon[])rearWeapons.Clone();
            shields = (Shield[])shields.Clone();
            generators = (Generator[])generators.Clone();
            leftSpecials = (SpecialWeapon[])leftSpecials.Clone();
            rightSpecials = (SpecialWeapon[])rightSpecials.Clone();
        }

        public object Clone() => new ShopStock(this);
    }
}