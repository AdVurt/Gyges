using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Gyges.Game;
using Gyges.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gyges.Menu {

    [RequireComponent(typeof(PlayerInput))]
    public class MainMenuUI : MonoBehaviour {

        private int _phase = 0;
        private int _itemSelected = 0;
        private float _menuTextAlpha = 0f;
        private float _lastKnownVerticalValue = 0f;
        private float _menuTextFadeInThreshold;
        private AudioSource _audioSource;
        // To make sure we don't play too many SFX in short succession
        private float _SFXTimer = 0f;

        [SerializeField] private RawImage _fadeInImage = default;
        [SerializeField] private AudioClip _music = default;
        [SerializeField] private RectTransform _logoTransform = default;
        [SerializeField] private SceneReference _shopScene = default;
        [SerializeField] private GameState _startingGameState = default;
        [SerializeField] private GameState _gameStateObject = default;
        [SerializeField] private Material _selectedMaterial = default;
        [SerializeField] private Material _unselectedMaterial = default;
        [SerializeField] private AudioClip _navigateSound = default;
        [SerializeField] private AudioClip _confirmSound = default;

        [Space]
        [SerializeField] private MainMenuItem[] _menuItems = default;
        private Rect[] _menuItemRects;

        [System.Serializable]
        public class MainMenuItem {
            public TextMeshProUGUI item;
            public UnityEvent itemLogic;
        }

        private PlayerInput _input;

        void Awake() {

            _input = GetComponent<PlayerInput>();
            _audioSource = GetComponent<AudioSource>();

            foreach(MainMenuItem text in _menuItems) {
                text.item.color = new Color(text.item.color.r, text.item.color.g, text.item.color.b, _menuTextAlpha);
                text.item.fontSharedMaterial = _unselectedMaterial;
            }
            _menuTextFadeInThreshold = _logoTransform.anchoredPosition.y / 2f;

            _menuItemRects = new Rect[_menuItems.Length];
            for(int i = 0; i < _menuItems.Length; i++) {
                _menuItemRects[i] = new Rect(_menuItems[i].item.rectTransform.position, _menuItems[i].item.rectTransform.sizeDelta);
                _menuItemRects[i].x -= _menuItemRects[i].width / 2;
            }

        }

        void OnEnable() {
            _input.actions["Vertical"].performed += OnNavigate;
            _input.actions["Vertical"].canceled += OnNavigate;
            _input.actions["Confirm"].performed += OnConfirm;
            _input.actions["Screen Position"].performed += MouseMoved;
            _input.actions["Screen Position"].Enable();
        }

        void OnDisable() {
            _input.actions["Vertical"].performed -= OnNavigate;
            _input.actions["Vertical"].canceled -= OnNavigate;
            _input.actions["Confirm"].performed -= OnConfirm;
            _input.actions["Screen Position"].performed -= MouseMoved;
        }



        private void MouseMoved(InputAction.CallbackContext context) {
            Vector2 pos = context.ReadValue<Vector2>();
            
            for (int i = 0; i < _menuItems.Length; i++) {
                if (_menuItemRects[i].Contains(pos) && _itemSelected != i) {
                    _itemSelected = i;
                    PlaySound(_navigateSound);
                    UpdateVisuals();
                }
            }

        }


        private void OnConfirm(InputAction.CallbackContext obj) {

            if (_phase == 2) {

                if (obj.control.device.name == "Mouse") {
                    Mouse mouse = (Mouse)obj.control.device;
                    if (_menuItemRects[_itemSelected].Contains(mouse.position.ReadValue())) {
                        PlaySound(_confirmSound);
                        _menuItems[_itemSelected].itemLogic.Invoke();
                    }
                }
                else {
                    PlaySound(_confirmSound);
                    _menuItems[_itemSelected].itemLogic.Invoke();
                }
            }

        }

        private void OnNavigate(InputAction.CallbackContext obj) {

            if (_phase == 2) {

                float yAxisVal = obj.ReadValue<float>();
                if (_lastKnownVerticalValue != yAxisVal) {
                    _lastKnownVerticalValue = yAxisVal;
                    if (yAxisVal == -1) {
                        _itemSelected++;
                        if (_itemSelected >= _menuItems.Length)
                            _itemSelected = 0;
                        UpdateVisuals();
                        PlaySound(_navigateSound);
                    }
                    else if (yAxisVal == 1) {
                        _itemSelected--;
                        if (_itemSelected < 0)
                            _itemSelected = _menuItems.Length - 1;
                        UpdateVisuals();
                        PlaySound(_navigateSound);
                    }
                }

            }
        }

        private void PlaySound(AudioClip audioClip) {
            if (_SFXTimer <= 0f) {
                _audioSource.PlayOneShot(audioClip, _audioSource.volume);
                _SFXTimer = 0.02f;
            }
        }

        void Start() {
            _fadeInImage.enabled = true;
            MusicManager.Play(_music);
        }

        // Update is called once per frame
        void Update() {

            if (_SFXTimer > 0f) {
                _SFXTimer -= Time.deltaTime;
            }

            switch (_phase) {

                //Fading in from black
                case 0:
                    _fadeInImage.color = new Color(_fadeInImage.color.r, _fadeInImage.color.g, _fadeInImage.color.b, _fadeInImage.color.a - Time.deltaTime);
                    if (_fadeInImage.color.a <= 0) {
                        _phase++;
                        UpdateVisuals();
                    }
                    break;

                case 1:
                    _logoTransform.anchoredPosition = Vector2.MoveTowards(_logoTransform.anchoredPosition, Vector2.zero, Time.deltaTime * 400f);
                    if (_logoTransform.anchoredPosition.y <= _menuTextFadeInThreshold) {
                        _menuTextAlpha = Mathf.Clamp01(_menuTextAlpha + Time.deltaTime);
                        foreach(MainMenuItem text in _menuItems) {
                            text.item.color = new Color(text.item.color.r, text.item.color.g, text.item.color.b, _menuTextAlpha);
                        }
                    }
                    if (_logoTransform.anchoredPosition == Vector2.zero && _menuTextAlpha >= 1f) {
                        _phase++;
                        UpdateVisuals();
                    }
                    break;

                case 2:
                    break;

            }
        }

        void UpdateVisuals() {
            switch (_phase) {
                case 2:
                    for (int i = 0; i < _menuItems.Length; i++) {
                        _menuItems[i].item.fontSharedMaterial = _itemSelected == i ? _selectedMaterial : _unselectedMaterial;
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Fade out to black and then go to level one (via a coroutine that is handled by this).
        /// </summary>
        public void GoToLevelOne() {
            _phase = -1;
            StartCoroutine(GoToL1Cort());
            
        }

        /// <summary>
        /// This coroutine fades out to black and then goes to level one.
        /// </summary>
        IEnumerator GoToL1Cort() {

            MusicManager.Play(null, 2f);
            float blackAlpha = 0f;
            while (blackAlpha < 1f) {
                blackAlpha += Time.deltaTime * 0.5f;
                _fadeInImage.color = new Color(_fadeInImage.color.r, _fadeInImage.color.g, _fadeInImage.color.b, blackAlpha);
                yield return new WaitForEndOfFrame();
            }
            _gameStateObject.CopyFrom(_startingGameState);
            _shopScene.LoadScene(LoadSceneMode.Single);
            yield return null;
        }

        public void QuitGame() {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }

}