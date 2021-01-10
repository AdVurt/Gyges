using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gyges.Game {
    public class PlayerHUD : MonoBehaviour {

        private Player _player;
        public Player Player {
            get {
                return _player;
            }
            set {
                if (_player != null && value != _player) {
                    UnsubscribeFromPlayer();
                }
                _player = value;
                if (_player != null) {
                    SubscribeToPlayer();
                    UpdatePower(_player.Power);
                    UpdateShields(_player.Shields);
                    UpdateHull(_player.Hull);
                }
                RefreshInfo();
            }
        }

        [SerializeField] private Color _activeColour = default;
        [SerializeField] private Color _inactiveColour = default;
        [SerializeField] private Image _background = default;

        [SerializeField] private GameObject[] _elements = default;
        [SerializeField] private TextMeshProUGUI _playerName = default;
        [SerializeField] private Image _powerBar = default;
        [SerializeField] private Image _shieldsBar = default;
        [SerializeField] private Image _hullBar = default;
        [SerializeField] private UILoadoutItem _frontWeaponSlot = default;
        [SerializeField] private UILoadoutItem _rearWeaponSlot = default;
        [SerializeField] private Image _rearWeaponAlt1 = default;
        [SerializeField] private Image _rearWeaponAlt2 = default;
        [SerializeField] private UILoadoutItem _leftSpecialSlot = default;
        [SerializeField] private UILoadoutItem _rightSpecialSlot = default;
        [SerializeField] private GameState _gameState = default;

        void SubscribeToPlayer() {
            _player.onPowerChanged += UpdatePower;
            _player.onShieldsChanged += UpdateShields;
            _player.GetComponent<Ship>().OnHealthChanged += UpdateHull;
            _player.onAltModeChanged += UpdateFireMode;
            UpdateFireMode(_player.AltFireMode);
        }

        void UnsubscribeFromPlayer() {
            if (_player == null)
                return;

            _player.onPowerChanged -= UpdatePower;
            _player.onShieldsChanged -= UpdateShields;
            _player.Ship.OnHealthChanged -= UpdateHull;
            _player.onAltModeChanged -= UpdateFireMode;
        }

        /// <summary>
        /// Refreshes the HUD area with all data except current power/shields/hull.
        /// Because this leads to UI rebuilds, it should not get called often.
        /// </summary>
        void RefreshInfo() {

            if (_player == null) {
                foreach (GameObject obj in _elements) {
                    if (obj.activeSelf)
                        obj.SetActive(false);
                }
                _background.color = _inactiveColour;
            }
            else {
                _playerName.text = "Player 1";
                _frontWeaponSlot.SetItem(_gameState.loadouts[_player.playerNumber].frontWeapon);
                _rearWeaponSlot.SetItem(_gameState.loadouts[_player.playerNumber].rearWeapon);
                _leftSpecialSlot.SetItem(_gameState.loadouts[_player.playerNumber].specialLeft);
                _rightSpecialSlot.SetItem(_gameState.loadouts[_player.playerNumber].specialRight);
                foreach (GameObject obj in _elements) {
                    if (!obj.activeSelf)
                        obj.SetActive(true);
                }
                _background.color = _activeColour;
            }
        }

        void UpdateFireMode(bool altMode) {
            if (_gameState.loadouts[_player.playerNumber].rearWeapon != null) {
                _rearWeaponAlt1.enabled = !altMode;
                _rearWeaponAlt2.enabled = altMode;
            }
            else {
                _rearWeaponAlt1.enabled = false;
                _rearWeaponAlt2.enabled = false;
            }
        }

        void UpdateBarValue(Image bar, float value, float maxValue) {
            //fillAmount does not require a UI rebuild, so this can be called every frame (which it might do).
            bar.fillAmount = value / maxValue;
        }

        void UpdatePower(float val) => UpdateBarValue(_powerBar, val, 100f);
        void UpdateShields(float val) => UpdateBarValue(_shieldsBar, val, 100f);
        void UpdateHull(float val) => UpdateBarValue(_hullBar, val, 100f);
    }
}