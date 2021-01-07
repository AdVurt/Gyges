using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gyges.Game {

    public class PartShopItem : MonoBehaviour {

        [Header("Do not manually set these values")]
        public ShipPart part;
        ///<summary>Is this the dummy "None" item?</summary>
        public bool dummyNullValue = false;
        public bool selected = false;
        public bool equipped = false;
        public bool canAffordUpgrade = false;
        public int levelDownAmount = -1;
        public int levelUpAmount = -1;
        public int level = 1;

        [Header("Set these values")]
        [SerializeField] private Material _unselectedMaterial = default;
        [SerializeField] private Material _selectedMaterial = default;
        [SerializeField] private TextMeshProUGUI _partName = default;
        [SerializeField] private TextMeshProUGUI _partCost = default;
        [SerializeField] private Image _equippedImage = default;
        [SerializeField] private Image _levelDownImage = default;
        [SerializeField] private TextMeshProUGUI _levelDownText = default;
        [SerializeField] private Image _levelUpImage = default;
        [SerializeField] private TextMeshProUGUI _levelUpText = default;

        /// <summary>
        /// Returns the (world-position) corners of the level-down image.
        /// </summary>
        public Vector3[] LevelDownIconCorners {
            get {
                Vector3[] result = new Vector3[4];
                _levelDownImage.rectTransform.GetWorldCorners(result);
                return result;
            }
        }

        /// <summary>
        /// Returns the (world-position) corners of the level-up image.
        /// </summary>
        public Vector3[] LevelUpIconCorners {
            get {
                Vector3[] result = new Vector3[4];
                _levelUpImage.rectTransform.GetWorldCorners(result);
                return result;
            }
        }

        public void RefreshVisuals() {

            if (part == null) {

                _levelDownImage.enabled = false;
                _levelDownText.enabled = false;
                _levelUpImage.enabled = false;
                _levelUpText.enabled = false;

                if (dummyNullValue) {
                    _partName.text = "None";
                    _partCost.text = "";
                    _equippedImage.enabled = equipped;
                }
                else {
                    _partName.text = "";
                    _partCost.text = "";
                    _equippedImage.enabled = false;
                }
            }
            else {
                _partName.text = part.inGameUIName;
                string totalCostText = $"Cost: {part.baseCost.ToString()}";
                _equippedImage.enabled = equipped;

                if (equipped && (part.ShipPartType == ShipPart.PartType.FrontWeapon || part.ShipPartType == ShipPart.PartType.RearWeapon) ) {

                    totalCostText = $"Level: {level}\tCost: {part.GetCost(level).ToString()}";

                    if (levelDownAmount > -1) {
                        _levelDownImage.enabled = true;
                        _levelDownText.text = levelDownAmount.ToString("###,###,###,###");
                        _levelDownText.enabled = true;
                    }
                    else {
                        _levelDownImage.enabled = false;
                        _levelDownText.enabled = false;
                    }

                    if (levelUpAmount > -1) {
                        
                        _levelUpImage.color = canAffordUpgrade ? Color.white : new Color(1f,1f,1f,0.25f);
                        _levelUpImage.enabled = true;
                        _levelUpText.text = levelUpAmount.ToString("###,###,###,###");
                        _levelUpText.enabled = true;
                    }
                    else {
                        _levelUpImage.enabled = false;
                        _levelUpText.enabled = false;
                    }
                    
                }
                else {
                    _levelDownImage.enabled = false;
                    _levelDownText.enabled = false;
                    _levelUpImage.enabled = false;
                    _levelUpText.enabled = false;
                }
                _partCost.text = totalCostText;
            }

            _partName.fontSharedMaterial = selected ? _selectedMaterial : _unselectedMaterial;
            _partCost.fontSharedMaterial = selected ? _selectedMaterial : _unselectedMaterial;

        }

        public override string ToString() {
            return $"Part: {part}, Dummy: {(dummyNullValue ? "TRUE" : "FALSE")}, Selected: {(selected ? "TRUE" : "FALSE")} " +
                $"Equipped: {(equipped ? "TRUE" : "FALSE")}";
        }

    }

}