using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gyges.Game {
    public class UILoadoutItem : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text = default;

        public void SetItem(ShipPart part) {
            
            if (part == null) {
                _text.text = "";
            }
            else {
                _text.text = part.inGameUIName;
            }

        }
    }
}