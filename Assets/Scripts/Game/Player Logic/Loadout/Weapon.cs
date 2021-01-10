using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {
    public abstract class Weapon : ShipPart {

        public WeaponLogic.WeaponType weaponType;

        [Tooltip("How fast will this oscillate?"), Range(0f, 5f)]
        public float oscillationSpeed = 0f;

        [Tooltip("The prefabs of this weapon's projectiles.")]
        public GameObject[] projectilePrefabs;

        [Tooltip("The sound that plays when this weapon is fired.")]
        public AudioClip fireSound;

    }

    public abstract class LevelableWeapon : Weapon {

        public const int maximumLevel = 11;

        public WeaponLogic[] levelStats = new WeaponLogic[11];

        public abstract bool HasAlternateFireMode { get; }

        /// <summary>
        /// Returns the cost of a specific level of this weapon. Not cumulative with previous levels.
        /// </summary>
        /// <param name="level">The level of this weapon.</param>
        public int LevelCost(int level) {
            if (level < 1 || level > maximumLevel)
                throw new System.ArgumentOutOfRangeException(nameof(level),$"Level must be between 1 and {maximumLevel} (inclusive).");

            int result = baseCost;
            int multiplier = 1;
            // Cost only starts increasing from level three.
            // The cost is increased by baseCost x 2 at level three, baseCost x 3 at level four, etc.
            for (int i = 3; i <= level; i++) {
                multiplier++;
                result += baseCost * multiplier;
            }
            return result;
        }

        public bool HasEnoughPower(int level, float currentPower) => levelStats[level - 1].powerCost <= currentPower;

        /// <summary>
        /// Returns the total (cumulative) cost of this weapon, given a level.
        /// </summary>
        /// <param name="level">The level of this weapon.</param>
        public override int GetCost(int level) {
            if (level < 1 || level > maximumLevel)
                throw new System.ArgumentOutOfRangeException(nameof(level), $"Error for {name}: Level must be between 1 and {maximumLevel} (inclusive). The given value was {level}.");

            int result = 0;

            for (int i = 1; i <= level; i++) {
                result += LevelCost(i);
            }

            return result;
        }

    }

    public abstract class AmmoWeapon : Weapon {

        [Tooltip("The maximum ammo capacity of this weapon.")]
        public uint maxAmmo = 0;
        [Tooltip("The amount of ammo that is required in order for this weapon to fire.")]
        public uint ammoRequirement = 0;
        [Tooltip("How much ammo should be recharged per second. If left at 0, this weapon will not automatically recharge.")]
        public float rechargeRate = 0f;
        public WeaponLogic logic = new WeaponLogic();

        public bool HasEnoughAmmo(int ammo) => ammo >= ammoRequirement;

    }
}