using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gyges.Utility {
    public static class GygesExtensions {

        public static Vector2 Rotate(this Vector2 vector, float degrees) {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y);

        }

    }
}