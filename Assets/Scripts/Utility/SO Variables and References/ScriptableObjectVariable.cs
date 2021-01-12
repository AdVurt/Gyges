using UnityEngine;

namespace Gyges.Utility {
    public abstract class ScriptableObjectVariable<T> : ScriptableObject {
        public T value;
    }
}