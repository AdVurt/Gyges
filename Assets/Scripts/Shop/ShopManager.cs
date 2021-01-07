using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Gyges.Utility;

namespace Gyges.Game {

    public class ShopManager : MonoBehaviour {

        public GameState gameState;
        public GameObject backgroundPrefab;
        [SerializeField] private AudioClip _shopMusic = default;
        [SerializeField] private PlayerShop _shop = default;
        [SerializeField] private RawImage _blackImage = default;
        [SerializeField] private SceneReference _mainMenu = default;

        // Start is called before the first frame update
        void Start() {
            //Create the star field, but have it play at quarter speed.
            Instantiate(backgroundPrefab).GetComponent<VisualEffect>().playRate = 0.25f;

            _blackImage.color = new Color(_blackImage.color.r, _blackImage.color.g, _blackImage.color.b, 1f);
            StartCoroutine(FadeInAndBegin());
            _shop.onBackToMenu += BackToMainMenu;
            _shop.onReady += Ready;
            MusicManager.Play(_shopMusic, 0f);

        }

        private IEnumerator FadeInAndBegin() {
            Color imageCol = _blackImage.color;

            while (imageCol.a > 0f) {
                imageCol.a -= Time.deltaTime;
                _blackImage.color = imageCol;
                yield return new WaitForEndOfFrame();
            }
            
            _shop.Enable();
            yield return null;
        }

        private void BackToMainMenu() {
            StartCoroutine(ReturnToMain());
        }

        private IEnumerator ReturnToMain() {
            Color imageCol = _blackImage.color;
            MusicManager.Stop();

            while (imageCol.a < 1f) {
                imageCol.a += Time.deltaTime;
                _blackImage.color = imageCol;
                yield return new WaitForEndOfFrame();
            }

            _mainMenu.LoadScene(UnityEngine.SceneManagement.LoadSceneMode.Single);
            yield return null;

        }

        private void Ready() {
            StartCoroutine(ReadyCoRt());
        }

        private IEnumerator ReadyCoRt() {
            Color imageCol = _blackImage.color;
            MusicManager.Stop();

            while (imageCol.a < 1f) {
                imageCol.a += Time.deltaTime;
                _blackImage.color = imageCol;
                yield return new WaitForEndOfFrame();
            }

            gameState.currentLevel = gameState.availableLevels[0];
            gameState.currentLevel.LoadLevel();
            yield return null;
        }
    }

}