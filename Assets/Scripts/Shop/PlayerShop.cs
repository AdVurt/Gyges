using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gyges.Utility;
using UnityEngine.InputSystem;
using System;
using System.Text;

namespace Gyges.Game {

    public class PlayerShop : MonoBehaviour {

        public int playerNumber = 0;
        private int _fundsRemaining = 0;
        private PlayerInput _input;
        private float _lastKnownHorizontalValue = 0f;
        private float _lastKnownVerticalValue = 0f;
        private ShipPart _currentItemInSlot;
        private Camera _mainCamera;
        private bool _altFire = false;

        [SerializeField] private TextMeshProUGUI _funds = default;
        [SerializeField] private ShopShipPreview _shipPreview = default;
        private AudioSource _audioSource;

        [Header("Object and Asset References")]
        public ShopShipPreview shipPreview;
        [SerializeField] private Material _unselectedTextMaterial = default;
        [SerializeField] private Material _selectedTextMaterial = default;
        [SerializeField] private TextMeshProUGUI _shopHeader = default;
        [SerializeField] private GameState _gameState = default;
        [SerializeField] private AudioClip _navigateSound = default;
        [SerializeField] private AudioClip _confirmSound = default;
        [SerializeField] private AudioClip _cancelSound = default;

        [Header("Main Shop Menu")]
        [SerializeField] private Canvas _mainMenuCanvas = default;
        [SerializeField] private TextMeshProUGUI _hull = default;
        [SerializeField] private TextMeshProUGUI _frontWeapon = default;
        [SerializeField] private TextMeshProUGUI _rearWeapon = default;
        [SerializeField] private TextMeshProUGUI _shield = default;
        [SerializeField] private TextMeshProUGUI _generator = default;
        [SerializeField] private TextMeshProUGUI _leftSpecial = default;
        [SerializeField] private TextMeshProUGUI _rightSpecial = default;
        [SerializeField] private TextMeshProUGUI _ready = default;
        [SerializeField] private TextMeshProUGUI _backToMainMenu = default;
        [SerializeField] private RectTransform[] _mainMenuRectTransforms = default;
        private Rect[] _mainMenuRects;

        [Header("Part Shop Menu")]
        [SerializeField] private Canvas _partShopCanvas = default;
        [SerializeField] private PartShopItem[] _partShopItems = default;
        [SerializeField] private TextMeshProUGUI _partShopBack = default;
        [SerializeField] private RectTransform[] _partShopRectTransforms = default;
        [SerializeField] private TextMeshProUGUI _rearModeText = default;
        private Rect[] _partShopRects;
        private readonly string[] _bindingDivider = new string[] { " | " };

        private bool _enabled = false;
        //Which phase we are currently in. There are two special phases, -1 (not yet in a phase), and 999 (leaving the shop)
        private int _phase = -1;
        private int _mainMenuSelected = 0;
        private int _partShopSelected = 0;
        //The number of parts in the current selected shop part type.
        private int _numberOfParts = 0;
        // To make sure we don't play too many SFX in short succession
        private float _SFXTimer = 0f;

        public event Action onReady;
        public event Action onBackToMenu;

        void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _input = GetComponent<PlayerInput>();
            shipPreview.PlayerNumber = playerNumber;
            _hull.fontSharedMaterial = _unselectedTextMaterial;
            _frontWeapon.fontSharedMaterial = _unselectedTextMaterial;
            _rearWeapon.fontSharedMaterial = _unselectedTextMaterial;
            _shield.fontSharedMaterial = _unselectedTextMaterial;
            _generator.fontSharedMaterial = _unselectedTextMaterial;
            _leftSpecial.fontSharedMaterial = _unselectedTextMaterial;
            _rightSpecial.fontSharedMaterial = _unselectedTextMaterial;
            _ready.fontSharedMaterial = _unselectedTextMaterial;
            _backToMainMenu.fontSharedMaterial = _unselectedTextMaterial;

            _mainMenuCanvas.enabled = true;
            _mainMenuRects = new Rect[_mainMenuRectTransforms.Length];
            for (int i = 0; i < _mainMenuRects.Length; i++) {
                _mainMenuRects[i] = GetWorldRect(_mainMenuRectTransforms[i]);
            }

            _partShopCanvas.enabled = false;
            _partShopRects = new Rect[_partShopRectTransforms.Length];
            for (int i = 0; i < _partShopRects.Length; i++) {
                _partShopRects[i] = GetWorldRect(_partShopRectTransforms[i]);
            }
        }

        void OnEnable() {
            _input.actions["Horizontal"].performed += OnHorizontalNavigate;
            _input.actions["Horizontal"].canceled += OnHorizontalNavigate;
            _input.actions["Vertical"].performed += OnVerticalNavigate;
            _input.actions["Vertical"].canceled += OnVerticalNavigate;
            _input.actions["Confirm"].performed += OnConfirm;
            _input.actions["Cancel"].performed += OnCancel;
            _input.actions["Change Rear Mode"].performed += OnRearModeChanged;
            _input.actions["Change Rear Mode"].Enable();
            _input.actions["Screen Position"].performed += MouseMoved;
            _input.actions["Screen Position"].Enable();

            _shipPreview.SetAltFireMode(_altFire);
        }

        void OnDisable() {
            _input.actions["Horizontal"].performed -= OnHorizontalNavigate;
            _input.actions["Horizontal"].canceled -= OnHorizontalNavigate;
            _input.actions["Vertical"].performed -= OnVerticalNavigate;
            _input.actions["Vertical"].canceled -= OnVerticalNavigate;
            _input.actions["Confirm"].performed -= OnConfirm;
            _input.actions["Cancel"].performed -= OnCancel;
            _input.actions["Change Rear Mode"].performed -= OnRearModeChanged;
            _input.actions["Screen Position"].performed -= MouseMoved;
        }

        void Start() {
            RefreshVisuals();
        }

        void Update() {
            if (_SFXTimer > 0f) {
                _SFXTimer -= Time.deltaTime;
            }

            if (_phase == 1 && _mainMenuSelected == 2) {
                // Rear weapon
                _rearModeText.enabled = _currentItemInSlot != null;

                StringBuilder sb = new StringBuilder(_altFire ? "Secondary" : "Primary");
                sb.AppendLine(" Firing Mode");
                sb.Append("Change Mode: ");

                string allBindings = _input.actions["Change Rear Mode"].GetBindingDisplayString(InputBinding.MaskByGroup(_input.currentControlScheme));

                bool atLeastOneBinding = false;
                foreach(string binding in allBindings.Split(_bindingDivider, StringSplitOptions.RemoveEmptyEntries)) {
                    if (atLeastOneBinding)
                        sb.Append(", ");

                    sb.Append('[');
                    switch (binding) {
                        case "MMB":
                            sb.Append("Middle Mouse");
                            break;

                        default:
                            sb.Append(binding);
                            break;
                    }
                    sb.Append(']');
                    
                    atLeastOneBinding = true;
                }

                _rearModeText.text = sb.ToString();
            }
            else {
                _rearModeText.enabled = false;
            }

        }

        public void Enable() {
            if (_enabled)
                return;

            _enabled = true;
            _phase = 0;
            RefreshVisuals();
        }

        private void PlaySound(AudioClip audioClip) {
            if (_SFXTimer <= 0f) {
                _audioSource.PlayOneShot(audioClip, _audioSource.volume);
                _SFXTimer = 0.02f;
            }
        }

        private Rect GetWorldRect(RectTransform rt) {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }

        private Rect GetWorldRect(Vector3[] v) {
            if (v.Length != 4)
                throw new ArgumentException("The provided array must be of length 4.");
            return new Rect(v[0], v[2] - v[0]);
        }

        private void MainMenuItemExecute(int menuItem) {

            switch (menuItem) {

                // Ship Loadout Items
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    _phase++;
                    RefreshItems(true);
                    RefreshVisuals(true);
                    PlaySound(_confirmSound);
                    break;

                // Ready
                case 7:
                    onReady?.Invoke();
                    shipPreview.movingOut = true;
                    _phase = 999;
                    PlaySound(_confirmSound);
                    break;

                // Back to Menu
                case 8:
                    onBackToMenu?.Invoke();
                    _phase = 999;
                    PlaySound(_cancelSound);
                    break;

            }

        }

        private void MouseMoved(InputAction.CallbackContext context) {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());

            switch (_phase) {
                // Main shop menu
                case 0:
                    for (int i = 0; i < _mainMenuRects.Length; i++) {
                        if (_mainMenuRects[i].Contains(worldPos)) {
                            bool updateVisuals = _mainMenuSelected != i;
                            _mainMenuSelected = i;
                            if (updateVisuals) {
                                RefreshVisuals();
                                PlaySound(_navigateSound);
                            }
                            break;
                        }
                    }
                    break;

                // Item shop
                case 1:
                    // Keep track of selection ID as it doesn't line up fully with the rects - if we don't have a full stock of items, this should skip over empty slots.
                    int selectionID = -1;
                    for (int i = 0; i < _partShopRects.Length; i++) {
                        //Skip checking empty slots.
                        if (i < _partShopItems.Length && _partShopItems[i].part == null && !_partShopItems[i].dummyNullValue) {
                            continue;
                        }
                        selectionID++;

                        if (_partShopRects[i].Contains(worldPos)) {
                            bool updateVisuals = _partShopSelected != selectionID;
                            _partShopSelected = selectionID;
                            if (updateVisuals) {
                                RefreshVisuals();
                                PlaySound(_navigateSound);
                            }
                            break;
                        }
                    }
                    break;
            }

        }


        private void OnRearModeChanged(InputAction.CallbackContext context) {
            if (_phase == 1 && _mainMenuSelected == 2) {
                _altFire = !_altFire;
                _shipPreview.SetAltFireMode(_altFire);
                RefreshVisuals(true);
            }
        }

        private void OnConfirm(InputAction.CallbackContext context) {
            
            //Main shop menu
            if (_phase == 0) {
                bool trigger = false;

                if (context.control.device.name == "Mouse") {
                    // Only count a mouse click if it clicked on the relevant UI item.
                    Mouse mouse = (Mouse)context.control.device;
                    if (_mainMenuRects[_mainMenuSelected].Contains((Vector2)_mainCamera.ScreenToWorldPoint(mouse.position.ReadValue()))) {
                        trigger = true;
                    }
                }
                else {
                    trigger = true;
                }

                if (trigger) {
                    MainMenuItemExecute(_mainMenuSelected);
                }
            }
            //Item shop menu
            else if (_phase == 1) {
                bool trigger = false;
                
                // If this was a mouse click was in the "level-up" or "level-down" icon range, this will be set to 1 or -1, respectively.
                int itemUpgradeAttempt = 0;

                if (context.control.device.name == "Mouse") {
                    // Only count a mouse click if it clicked on the relevant UI item.
                    Mouse mouse = (Mouse)context.control.device;
                    Vector2 worldPos = _mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
                    //We need a separate position tracker to support "skipping" over empty UI items.
                    int positionID = -1;
                    for (int i = 0; i < _partShopRects.Length; i++) {
                        //Skip checking empty slots.
                        if (i < _partShopItems.Length && _partShopItems[i].part == null && !_partShopItems[i].dummyNullValue) {
                            continue;
                        }
                        positionID++;

                        if (_partShopRects[i].Contains(worldPos)) {
                            trigger = _partShopSelected == positionID;

                            if (i < _partShopItems.Length) {
                                if (GetWorldRect(_partShopItems[i].LevelDownIconCorners).Contains(worldPos)) {
                                    itemUpgradeAttempt = -1;
                                }
                                else if (GetWorldRect(_partShopItems[i].LevelUpIconCorners).Contains(worldPos)) {
                                    itemUpgradeAttempt = 1;
                                }
                            }
                            break;
                        }
                    }

                }
                else {
                    trigger = true;
                }

                if (trigger) {

                    //Back button
                    if (_partShopSelected == _numberOfParts) {
                        _phase = 0;
                        PlaySound(_cancelSound);
                    }
                    //Item buy
                    else {
                        if (_partShopItems[_partShopSelected].equipped) {
                            //This is already equipped, check to see if an upgrade or downgrade is being attempted.

                            if ((_mainMenuSelected == 1 || _mainMenuSelected == 2) && _partShopItems[_partShopSelected].part == _currentItemInSlot && _currentItemInSlot != null) {

                                ref int levelRef = ref GetLoadoutTypeLevelRef();

                                if (itemUpgradeAttempt == -1 && levelRef > 1) {
                                    //Downgrade
                                    levelRef--;
                                    shipPreview.Refresh();
                                    RefreshVisuals();
                                    PlaySound(_navigateSound);
                                }
                                else if (itemUpgradeAttempt == 1 && levelRef < LevelableWeapon.maximumLevel) {
                                    //Upgrade (if we can afford it).
                                    if (_fundsRemaining >= ((LevelableWeapon)_currentItemInSlot).LevelCost(levelRef + 1)) {
                                        levelRef++;
                                        shipPreview.Refresh();
                                        RefreshVisuals();
                                    }
                                    PlaySound(_navigateSound);
                                }

                            }

                        }
                        else {
                            //Check if the player can afford the item.
                            Loadout loadout = _gameState.loadouts[playerNumber];
                            int currentPartCost = Loadout.GetTotalValueOfParts(loadout.frontWeaponLevel, loadout.rearWeaponLevel, _currentItemInSlot);

                            //If the player is nulling out the item, they can always afford it.
                            if (_partShopItems[_partShopSelected].part == null) {
                                _fundsRemaining += currentPartCost;
                                Equip(null, _mainMenuSelected);
                                shipPreview.Refresh();
                                PlaySound(_confirmSound);
                            }
                            else if (_fundsRemaining + currentPartCost >= _partShopItems[_partShopSelected].part.baseCost) {
                                _fundsRemaining += currentPartCost;
                                _fundsRemaining -= _partShopItems[_partShopSelected].part.baseCost;
                                Equip(_partShopItems[_partShopSelected].part, _mainMenuSelected);
                                shipPreview.Refresh();
                                PlaySound(_confirmSound);
                            }

                        }
                    }

                    
                }
                RefreshVisuals();
            }

        }

        private void Equip(ShipPart part, int slot) {

            Loadout loadout = _gameState.loadouts[playerNumber];

            switch (slot) {
                case 0:
                    loadout.hull = (Hull)part;
                    break;
                case 1:
                    loadout.frontWeapon = (FrontWeapon)part;
                    loadout.frontWeaponLevel = 1;
                    break;
                case 2:
                    loadout.rearWeapon = (RearWeapon)part;
                    loadout.rearWeaponLevel = 1;
                    break;
                case 3:
                    loadout.shield = (Shield)part;
                    break;
                case 4:
                    loadout.generator = (Generator)part;
                    break;
                case 5:
                    loadout.specialLeft = (SpecialWeapon)part;
                    break;
                case 6:
                    loadout.specialRight = (SpecialWeapon)part;
                    break;
            }

        }

        private void OnCancel(InputAction.CallbackContext context) {

            if (_phase == 1) {
                _phase = 0;
                RefreshVisuals();
                PlaySound(_cancelSound);
            }

        }

        private void OnVerticalNavigate(InputAction.CallbackContext context) {

            //Main shop menu
            if (_phase == 0) {
                Navigate(ref _lastKnownVerticalValue, ref _mainMenuSelected, 0, 9, -context.ReadValue<float>(), true);
            }
            //Ship part menu
            else if (_phase == 1) {
                Navigate(ref _lastKnownVerticalValue, ref _partShopSelected, 0, _numberOfParts + 1, -context.ReadValue<float>(), true);
            }

        }

        private void OnHorizontalNavigate(InputAction.CallbackContext context) {
            
            // If we're in the part shop, and the current part is equipped...
            if (_phase == 1 && (_mainMenuSelected == 1 || _mainMenuSelected == 2) && _partShopItems[_partShopSelected].part == _currentItemInSlot && _currentItemInSlot != null) {

                ref int correctLevel = ref GetLoadoutTypeLevelRef();

                //Force the value not to change if the player can't afford the upgrade.
                bool canUpdate = false;

                if (correctLevel < LevelableWeapon.maximumLevel && _fundsRemaining >= ((LevelableWeapon)_currentItemInSlot).LevelCost(correctLevel + 1)) {
                    canUpdate = true;
                }

                int chgAmt = Navigate(ref _lastKnownHorizontalValue, ref correctLevel, 1, canUpdate ? LevelableWeapon.maximumLevel+1 : correctLevel, context.ReadValue<float>(), false);
                if (chgAmt != 0) {
                    RefreshItems();
                    shipPreview.Refresh();
                }
            }

        }

        private ref int GetLoadoutTypeLevelRef() {
            Loadout loadout = _gameState.loadouts[playerNumber];

            if (_mainMenuSelected == 1) {
                return ref loadout.frontWeaponLevel;
            }
            else if (_mainMenuSelected == 2) {
                return ref loadout.rearWeaponLevel;
            }
            else {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles vertical or horizontal navigation for a given input.
        /// </summary>
        /// <param name="tracker">The field that tracks the last known position of the input axis.</param>
        /// <param name="variable">The field to be incremented or decremented based on the input axis.</param>
        /// <param name="lowerLimit">The lower limit to clamp the variable value to. It will not be allowed to go below this value.</param>
        /// <param name="upperLimit">The upper limit to clamp the variable value to. It will not be allowed to reach this value.</param>
        /// <param name="axis">The input axis value.</param>
        /// <param name="wrap">Whether to wrap the value around if the upper or lower limits are exceeded. If false, instead there will be no change.</param>
        /// <returns>How much the variable value was changed by.</returns>
        private int Navigate(ref float tracker, ref int variable, int lowerLimit, int upperLimit, float axis, bool wrap) {

            int chg = variable;

            if (tracker != axis) {
                tracker = axis;
                if (axis == 1) {

                    variable++;

                    if (variable >= upperLimit) {
                        if (wrap) {
                            variable = lowerLimit;
                        }
                        else {
                            variable--;
                        }
                    }
                    RefreshVisuals();
                    PlaySound(_navigateSound);
                }
                else if (axis == -1) {
                    variable--;
                    if (variable < lowerLimit) {
                        if (wrap) {
                            variable = upperLimit - 1;
                        }
                        else {
                            variable++;
                        }
                    }
                    RefreshVisuals();
                    PlaySound(_navigateSound);
                }
            }

            return chg - variable;
        }

        /// <summary>
        /// Update the visual state of every UI element in the shop.
        /// </summary>
        private void RefreshVisuals(bool alreadyRefreshedItemState = false) {

            switch (_phase) {
                // Main shop menu
                case 0:
                    _mainMenuCanvas.enabled = true;
                    _partShopCanvas.enabled = false;
                    _shopHeader.text = "Shop";
                    UpdateTextItemVisual(_hull, _mainMenuSelected == 0);
                    UpdateTextItemVisual(_frontWeapon, _mainMenuSelected == 1);
                    UpdateTextItemVisual(_rearWeapon, _mainMenuSelected == 2);
                    UpdateTextItemVisual(_shield, _mainMenuSelected == 3);
                    UpdateTextItemVisual(_generator, _mainMenuSelected == 4);
                    UpdateTextItemVisual(_leftSpecial, _mainMenuSelected == 5);
                    UpdateTextItemVisual(_rightSpecial, _mainMenuSelected == 6);
                    UpdateTextItemVisual(_ready, _mainMenuSelected == 7);
                    UpdateTextItemVisual(_backToMainMenu, _mainMenuSelected == 8);
                    break;

                // Part shop menu
                case 1:
                    _mainMenuCanvas.enabled = false;
                    _partShopCanvas.enabled = true;
                    if (!alreadyRefreshedItemState)
                        RefreshItems();
                    UpdateTextItemVisual(_partShopBack, _partShopSelected == _numberOfParts);
                    break;
            }

            _fundsRemaining = _gameState.totalPoints - _gameState.loadouts[playerNumber].GetTotalValue();
            _funds.text = $"Funds: {_fundsRemaining.ToString("###,###,###,##0")}";

        }

        /// <summary>
        /// Refreshes the current item data, based on the selected (main menu-level) item type.
        /// </summary>
        /// <param name="selectEquipped">If true, the current equipped item will also be highlighted.</param>
        /// <returns>The ID (list index) of the item that is currently equipped.</returns>
        private int RefreshItems(bool selectEquipped = false) {
            Loadout loadout = _gameState.loadouts[playerNumber];
            int equipped = 0;
            if (selectEquipped)
                _partShopSelected = -1;

            switch (_mainMenuSelected) {
                case 0:
                    _shopHeader.text = "Hull";
                    _currentItemInSlot = loadout.hull;
                    break;
                case 1:
                    _shopHeader.text = "Front Weapon";
                    _currentItemInSlot = loadout.frontWeapon;
                    break;
                case 2:
                    _shopHeader.text = "Rear Weapon";
                    _currentItemInSlot = loadout.rearWeapon;
                    break;
                case 3:
                    _shopHeader.text = "Shield";
                    _currentItemInSlot = loadout.shield;
                    break;
                case 4:
                    _shopHeader.text = "Generator";
                    _currentItemInSlot = loadout.generator;
                    break;
                case 5:
                    _shopHeader.text = "Left Special";
                    _currentItemInSlot = loadout.specialLeft;
                    break;
                case 6:
                    _shopHeader.text = "Right Special";
                    _currentItemInSlot = loadout.specialRight;
                    break;
                default:
                    _currentItemInSlot = null;
                    break;
            }
            PopulatePartsShop(GetCurrentShipParts(), _mainMenuSelected == 2 || _mainMenuSelected == 5 || _mainMenuSelected == 6);
            for (int i = 0; i < _partShopItems.Length; i++) {

                int level = 1;
                int upCost = -1;
                int downCost = -1;
                bool canAfford = false;

                if ((_partShopItems[i].part != null && _partShopItems[i].part == _currentItemInSlot) ||
                    (_partShopItems[i].part == null && _partShopItems[i].dummyNullValue && _currentItemInSlot == null)) {
                    _partShopItems[i].equipped = true;

                    // Level-up/level-down info for front and rear weapons that are equipped.
                    if (_partShopItems[i].part != null && (_mainMenuSelected == 1 || _mainMenuSelected == 2)) {
                        level = _mainMenuSelected == 1 ? loadout.frontWeaponLevel : loadout.rearWeaponLevel;
                        LevelableWeapon item = (LevelableWeapon)_currentItemInSlot;

                        if (level < LevelableWeapon.maximumLevel) {
                            upCost = item.LevelCost(level+1);
                            canAfford = _fundsRemaining >= upCost;
                        }

                        if (level > 1) {
                            downCost = item.GetCost(level) - item.LevelCost(level - 1);
                        }
                    }

                    equipped = i;
                    if (selectEquipped) {
                        _partShopSelected = i;
                    }
                }
                else {
                    _partShopItems[i].equipped = false;
                }
                _partShopItems[i].selected = _partShopSelected == i;

                _partShopItems[i].levelUpAmount = upCost;
                _partShopItems[i].levelDownAmount = downCost;
                _partShopItems[i].canAffordUpgrade = canAfford;
                _partShopItems[i].level = level;

                _partShopItems[i].RefreshVisuals();
            }
            return equipped;
        }

        /// <summary>
        /// Get the current array of in-stock ship parts.
        /// </summary>
        private ShipPart[] GetCurrentShipParts() {
            if (_phase != 1)
                return null;

            ShopStock stock = _gameState.shopStock;
            switch (_mainMenuSelected) {
                case 0:
                    return stock.hulls;
                case 1:
                    return stock.frontWeapons;
                case 2:
                    return stock.rearWeapons;
                case 3:
                    return stock.shields;
                case 4:
                    return stock.generators;
                case 5:
                    return stock.leftSpecials;
                case 6:
                    return stock.rightSpecials;
            }

            return null;
        }

        /// <summary>
        /// Populate the ship part objects with the provided array of ship parts.
        /// </summary>
        /// <param name="parts">The array of parts.</param>
        /// <param name="addDummyValue">Whether or not to add a dummy "None" part.</param>
        private void PopulatePartsShop(ShipPart[] parts, bool addDummyValue) {
            _numberOfParts = -1;
            bool doneDummy = false;

            for (int i = 0; i < _partShopItems.Length; i++) {
                if (parts.Length > i) {
                    _partShopItems[i].part = parts[i];
                }
                else {
                    _partShopItems[i].part = null;
                    if (_numberOfParts == -1) {
                        _numberOfParts = i;
                    }

                    if (addDummyValue && !doneDummy) {
                        _partShopItems[i].dummyNullValue = true;
                        _numberOfParts++;
                        doneDummy = true;
                    }
                    else {
                        _partShopItems[i].dummyNullValue = false;
                    }

                }   
            }

            if (_numberOfParts == -1) {
                _numberOfParts = _partShopItems.Length;
            }
        }

        private void UpdateTextItemVisual(TextMeshProUGUI textItem, bool selected) {
            textItem.fontSharedMaterial = selected ? _selectedTextMaterial : _unselectedTextMaterial;
        }

    }

}