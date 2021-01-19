using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

namespace Gyges.Game {

    public class InGameUI : MonoBehaviour {

        private static string _message = "";
        private static float _messageTimer = 0f;

        private Player _player;
        private Transform _playerTransform;
        private float _delayPerLetter = 0.1f;
        private AudioSource _audioSource;

        [Header("Level-Specific")]
        [SerializeField] private Vector3 _playerStartPosition = new Vector3(0f,-4f,0f);

        [Header("Overlays")]
        [SerializeField] private GameObject _blackScreen = default;
        private Image _blackScreenImage;
        [SerializeField] private TextMeshProUGUI _topBlackScreenText = default;
        [SerializeField] private TextMeshProUGUI _bottomBlackScreenText = default;
        [SerializeField] private Image _alertImage = default;
        [SerializeField] private TextMeshProUGUI _messageText = default;
        // Alert Logic (0 = No alert, 1 = Raising, 2 = Lowering)
        private bool _alertPhase = false;

        [Header("Side panels")]
        [SerializeField] private RectTransform _leftPanel = default;
        [SerializeField] private PlayerHUD _leftHUD = default;
        [SerializeField] private RectTransform _rightPanel = default;
        [SerializeField] private PlayerHUD _rightHUD = default;

        [Header("Audio")]
        [SerializeField] private AudioClip _dialogueBleep = default;

        [Header("Settings")]
        [SerializeField] private GameplaySettings _gameplaySettings = default;
        [SerializeField] private GameState _gameState = default;

        void Awake() {
            _player = FindObjectOfType<Player>();
            _playerTransform = _player.GetComponent<Transform>();
            _blackScreenImage = _blackScreen.GetComponent<Image>();
            _audioSource = GetComponent<AudioSource>();
            Global.enableGameLogic = false;
            _messageText.text = "";
            _messageText.color = new Color(1f,1f,1f,0f);
        }

        void Start() {
            SetMessage("",0f);
            _blackScreen.SetActive(true);
            StartCoroutine(LevelIntro());

#if UNITY_EDITOR
            if (_gameState.currentLevel == null || _gameState.currentLevel.scene.sceneName != SceneManager.GetActiveScene().name) {
                Debug.LogWarning("The current game state does not point towards the current level.");
            }
            if (_gameState.currentLevel != null && _gameState.currentLevel.startingMusic == null) {
                Debug.LogWarning("No music has been assigned for this level.");
            }
#endif

        }

        void OnEnable() {
            _gameplaySettings.OnP1HUDUpdate += UpdateHUDMapping;
            _gameplaySettings.OnStreamerModeUpdate += UpdateStreamerMode;
        }

        void OnDisable() {
            _gameplaySettings.OnP1HUDUpdate -= UpdateHUDMapping;
            _gameplaySettings.OnStreamerModeUpdate -= UpdateStreamerMode;
        }

        void Update() {

            if (_alertPhase) {
                float alpha = _alertImage.color.a - Time.deltaTime;
                if (alpha <= 0f) {
                    alpha = 0f;
                    _alertPhase = false;
                }
                _alertImage.color = new Color(_alertImage.color.r, _alertImage.color.g, _alertImage.color.b, alpha);
            }
            else {

                bool alert = false;

                void SetAlert(Player p) {
                    if (p.Hull <= 20f && p.Hull <= p.CurrentLoadout.hull.startingHull) {
                        alert = true;
                        
                    }
                }
                
                if (_leftHUD.Player != null) {
                    SetAlert(_leftHUD.Player);
                }
                if (!alert && _rightHUD.Player != null) {
                    SetAlert(_rightHUD.Player);
                }

                if (alert) {
                    float alpha = _alertImage.color.a + Time.deltaTime;
                    if (alpha >= 1f) {
                        alpha = 1f;
                        _alertPhase = true;
                    }
                    _alertImage.color = new Color(_alertImage.color.r, _alertImage.color.g, _alertImage.color.b, alpha);
                }
            }


            if (_messageTimer > 0f) {
                _messageTimer -= Time.deltaTime;
                if (_messageTimer <= 0f) {
                    _message = "";
                }
            }

            // If the visual text is null:
            if (string.IsNullOrEmpty(_messageText.text)) {
                if (_message != "") {
                    _messageText.text = _message;
                }
                else {
                    
                }
            }
            else {

                Color textCol = _messageText.color;

                // If there should be visual text:
                if (_message == "") {
                    // Fade out if necessary.
                    if (textCol.a > 0f) {
                        textCol.a -= Time.deltaTime;
                        if (textCol.a < 0f) {
                            textCol.a = 0f;
                            _messageText.text = "";
                        }
                        _messageText.color = textCol;
                    }
                }
                else {
                    // Fade in if necessary.
                    if (textCol.a < 1f) {
                        textCol.a += Time.deltaTime;
                        if (textCol.a > 1f)
                            textCol.a = 1f;
                        _messageText.color = textCol;
                    }
                }

            }

        }

        IEnumerator LevelIntro() {

            if (MusicManager.IsPlaying()) {
                MusicManager.Play(null, 3f);
            }

            string targetText = $"Now entering {_gameState.currentLevel?.inGameName}";
            StringBuilder sb = new StringBuilder();
            _topBlackScreenText.text = "";
            _bottomBlackScreenText.text = "";
            yield return new WaitForSeconds(_delayPerLetter * 2f);

            for (int i = 0; i < targetText.Length; i++) {
                sb.Append(targetText[i]);
                _topBlackScreenText.text = sb.ToString();
                if (targetText[i] != ' ') {
                    _audioSource.PlayOneShot(_dialogueBleep);
                }
                yield return new WaitForSeconds(_delayPerLetter);
            }

            yield return new WaitForSeconds(_delayPerLetter);
            targetText = "Good luck!";
            sb.Clear();
            for (int i = 0; i < targetText.Length; i++) {
                sb.Append(targetText[i]);
                _bottomBlackScreenText.text = sb.ToString();
                if (targetText[i] != ' ') {
                    _audioSource.PlayOneShot(_dialogueBleep);
                }
                yield return new WaitForSeconds(_delayPerLetter);
            }
            yield return new WaitForSeconds(_delayPerLetter);

            MusicManager.Play(_gameState.currentLevel?.startingMusic, 0f);
            float alpha = _blackScreenImage.color.a;
            while (alpha > 0f) {
                alpha -= Time.deltaTime;
                _blackScreenImage.color = new Color(0f,0f,0f,alpha);
                yield return new WaitForEndOfFrame();
            }

            while (_playerTransform.position != _playerStartPosition) {
                _playerTransform.position = Vector3.MoveTowards(_playerTransform.position, _playerStartPosition, Time.deltaTime * Mathf.Max(0.5f,Vector3.Distance(_playerTransform.position, _playerStartPosition)*2f) );
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.1f);
            _player.enabled = true;
            Global.enableGameLogic = true;

            StartCoroutine(FadeTextOut());

            UpdateHUDMapping(_gameplaySettings.P1HUDOnRight);
            UpdateStreamerMode(_gameplaySettings.StreamerMode);

            while (_leftPanel.localScale != Vector3.one) {
                _leftPanel.localScale = Vector3.MoveTowards(_leftPanel.localScale, Vector3.one, Time.deltaTime);
                _rightPanel.localScale = Vector3.MoveTowards(_rightPanel.localScale, Vector3.one, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

        }

        IEnumerator FadeTextOut() {

            Vector3 col = new Vector3(_topBlackScreenText.color.r, _topBlackScreenText.color.g, _topBlackScreenText.color.b);
            float alpha = _topBlackScreenText.color.a;
            while (alpha > 0f) {
                alpha -= Time.deltaTime;
                _topBlackScreenText.color = new Color(col.x, col.y, col.z,alpha);
                _bottomBlackScreenText.color = new Color(col.x, col.y, col.z, alpha);
                yield return new WaitForEndOfFrame();
            }
            _blackScreen.SetActive(false);

        }

        void UpdateHUDMapping(bool value) {
            _rightHUD.Player = value ? _player : null;
            _leftHUD.Player = value ? null : _player;
        }

        void UpdateStreamerMode(bool value) {
            if (value) {
                //Anchor to top corners with Y offset of 0
                _leftPanel.anchorMin = new Vector2(0f, 1f);
                _leftPanel.anchorMax = _leftPanel.anchorMin;
                _leftPanel.pivot = new Vector2(0f, 1f);
                _rightPanel.anchorMin = new Vector2(1f, 1f);
                _rightPanel.anchorMax = _rightPanel.anchorMin;
                _rightPanel.pivot = new Vector2(1f, 1f);
            }
            else {
                //Anchor to horizontal edge middles with Y offset of 0
                _leftPanel.anchorMin = new Vector2(0f, 0.5f);
                _leftPanel.anchorMax = _leftPanel.anchorMin;
                _leftPanel.pivot = new Vector2(0f, 0.5f);
                _rightPanel.anchorMin = new Vector2(1f, 0.5f);
                _rightPanel.anchorMax = _rightPanel.anchorMin;
                _rightPanel.pivot = new Vector2(1f, 0.5f);
            }

            
        }

        public static void SetMessage(string message, float messageTimer) {
            _message = message;
            _messageTimer = messageTimer;
        }

    }

}
