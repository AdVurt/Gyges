using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gyges.Game {
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour {

        private static MusicManager _instance;
        private static MusicManager Instance {
            get {
                if (_instance == null) {
                    _instance = Instantiate(Resources.Load<GameObject>("Prefabs/Music Manager")).GetComponent<MusicManager>();
                }
                return _instance;
            }
            set { _instance = value; }
        }

        private AudioSource _audioSource;
        private float _startingVolume;

        private bool _fadingOut = false;

        void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _startingVolume = _audioSource.volume;
        }

        void OnEnable() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable() {
            if (_instance == this)
                _instance = null;
        }

        public static bool IsPlaying() {
            return Instance._audioSource.isPlaying;
        }

        /// <summary>
        /// Loops the provided music clip, first fading out over a specified amount of seconds.
        /// </summary>
        /// <param name="music">The clip to loop (if null is provided, no music will play after the fade out).</param>
        /// <param name="delay">How long to fade the current music out over. If it is set to 0, or if there is no current music, it will play instantly.</param>
        public static void Play(AudioClip music, float delay = 1.0f, bool loop = true) {
            Instance.PlayClip(music, delay, loop);
        }

        /// <summary>
        /// Plays the intro music and then loops the provided music clip, first fading out over a specified amount of seconds.
        /// </summary>
        /// <param name="music">The clip to loop (if null is provided, no music will play after the fade out).</param>
        /// <param name="delay">How long to fade the current music out over. If it is set to 0, or if there is no current music, it will play instantly.</param>
        public static void Play(AudioClip intro, AudioClip music, float delay = 1.0f, bool loop = true) {
            Instance.PlayClip(intro, music, delay, loop);
        }

        /// <summary>
        /// Stops the current music (if it is playing), fading out over a specified amount of seconds.
        /// This is the same as calling Play with a null value for the first parameter.
        /// </summary>
        /// <param name="delay">How long to fade the current music out over. If it is set to 0, it will stop instantly.</param>
        public static void Stop(float delay = 1.0f) => Instance.PlayClip(null, delay);

        private void PlayClip(AudioClip music, float delay = 1.0f, bool loop = true) {
            if (!_fadingOut) {
                _fadingOut = true;
            }
            else {
                StopCoroutine("FadeThenPlay");
            }

            StartCoroutine(FadeThenPlay(new AudioClip[] { music }, delay, loop));
            _fadingOut = true;
        }


        private void PlayClip(AudioClip intro, AudioClip music, float delay = 1.0f, bool loop = true) {
            if (!_fadingOut) {
                _fadingOut = true;
            }
            else {
                StopCoroutine("FadeThenPlay");
            }

            StartCoroutine(FadeThenPlay(new AudioClip[] { intro, music }, delay, loop));
        }

        private IEnumerator FadeThenPlay(AudioClip[] music, float delay, bool loop = true) {
            float fadeTimer = _audioSource.isPlaying ? delay : 0f;
            while (fadeTimer > 0f) {

                fadeTimer -= Time.deltaTime;
                _audioSource.volume = (fadeTimer / delay) * _startingVolume;

                yield return new WaitForEndOfFrame();
            }

            _fadingOut = false;
            _audioSource.Stop();
            _audioSource.volume = _startingVolume;
            if (music.Length == 1 && music[0] != null) {
                _audioSource.clip = music[0];
                _audioSource.loop = loop;
                _audioSource.Play();
            }
            else if (music.Length == 2 && music[0] != null && music[1] != null) {
                _audioSource.PlayOneShot(music[0]);
                _audioSource.clip = music[1];
                _audioSource.loop = loop;
                _audioSource.PlayScheduled(AudioSettings.dspTime + music[0].length);
            }
        }

    }
}