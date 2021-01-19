using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Utility {

    public class ReadOnlyAttribute : PropertyAttribute {
        public bool editableInEditMode;

        public ReadOnlyAttribute(bool editableInEditMode = false) {
            this.editableInEditMode = editableInEditMode;
        }
    }

}