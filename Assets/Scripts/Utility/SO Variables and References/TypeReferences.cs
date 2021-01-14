using System;
using UnityEngine;

namespace Gyges.Utility {

    [Serializable]
    public abstract class VariableReference<BasicType> {
        public bool useConstant = false;
        public BasicType constantValue;
        public ScriptableObjectVariable<BasicType> variable;

        public BasicType Value {
            get {
                return useConstant ? constantValue : variable.value;
            }
        }

        public VariableReference() {
            
        }

        public VariableReference(BasicType constant) {
            useConstant = true;
            constantValue = constant;
        }

        public VariableReference(ScriptableObjectVariable<BasicType> variable) {
            useConstant = false;
            this.variable = variable;
        }

    }

    [Serializable] public class BoolReference : VariableReference<bool> { }
    [Serializable] public class IntReference : VariableReference<int> { }
    [Serializable] public class FloatReference : VariableReference<float> { }
    [Serializable] public class ColourReference : VariableReference<Color> { }
    [Serializable] public class StringReference : VariableReference<string> { }
    [Serializable] public class Vector2Reference : VariableReference<Vector2> { }
    [Serializable] public class Vector3Reference : VariableReference<Vector3> { }
}