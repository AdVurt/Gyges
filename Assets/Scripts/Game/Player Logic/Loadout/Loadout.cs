using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges.Game {

    [Serializable]
    public class Loadout {

        public FrontWeapon frontWeapon;
        public RearWeapon rearWeapon;
        public Generator generator;
        public Hull hull;
        public Shield shield;
        public SpecialWeapon specialLeft;
        public SpecialWeapon specialRight;

        [Range(1,11)]
        public int frontWeaponLevel = 1;
        [Range(1,11)]
        public int rearWeaponLevel = 1;

        public event Action update;

        /// <summary>
        /// Returns the total monetary value of this loadout.
        /// </summary>
        public int GetTotalValue() => GetTotalValueOfParts(frontWeaponLevel, rearWeaponLevel,
                new ShipPart[] { hull, frontWeapon, rearWeapon, shield, generator, specialLeft, specialRight }
                );

        /// <summary>
        /// Returns the total base monetary value of the given parts.
        /// Requires front weapon and rear weapon level values.
        /// </summary>
        public static int GetTotalValueOfParts(int frontLevel, int rearLevel, params ShipPart[] parts) {
            int result = 0;
            foreach (ShipPart part in parts) {
                if (part == null)
                    continue;

                if (part.ShipPartType == ShipPart.PartType.FrontWeapon) {
                    result += part.GetCost(frontLevel);
                }
                else if (part.ShipPartType == ShipPart.PartType.RearWeapon) {
                    result += part.GetCost(rearLevel);
                }
                else {
                    result += part.GetCost();
                }
            }
            return result;
        }

        /// <summary>
        /// Call this to trigger the event where subscribed classes should perform some
        /// task in response to the loadout being updated.
        /// </summary>
        public void OnUpdate() {
            update?.Invoke();
        }

        public override string ToString() {
            return $"Loadout - Front: {(frontWeapon == null ? "NULL" : frontWeapon.name)}, Rear: {(rearWeapon == null ? "NULL" : rearWeapon.name)}, " + 
                $"Generator: {generator.name}, Hull: {hull.name}, Shield: {shield.name}, Special (L): {(specialLeft == null ? "NULL" : specialLeft.name)}, " +
                $"Special (R): {(specialRight == null ? "NULL" : specialRight.name)}";
        }
    }

}