using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gyges.Game {
    [System.Serializable]
    public class WeaponLogic {
        public enum WeaponType {
            FixedLaunch,
            OscillatingLaunch
        }

        [Tooltip("Power cost of firing all projectiles.")]
        public float powerCost = 0f;

        [Tooltip("The amount of time (in seconds) that must elapse after firing before this weapon can fire again.")]
        public float reloadTime = 0.2f;

        [Tooltip("Damage per individual projectile.")]
        public float damage = 5f;

        public ProjectileFormation formation = ProjectileFormation.SingleStraight;

    }
}