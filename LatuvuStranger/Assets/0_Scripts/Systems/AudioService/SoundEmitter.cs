using System.Collections;
using Latuvu;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Padrox.Acelab.Modules.Audio {
    /// <summary>
    /// Credits: <see href="https://www.youtube.com/@git-amend">git-amend</see>
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour {
        public SoundData Data { get; private set; }
        private AudioSource _audioSource;
        private Coroutine _playingCoroutine;

        private void Awake() {
            _audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        public void Play() {
            if (_playingCoroutine != null) {
                StopCoroutine(_playingCoroutine);
            }

            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        private IEnumerator WaitForSoundToEnd() {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            SoundController.Instance.ReturnToPool(this);
        }

        public void Stop() {
            if (_playingCoroutine != null) {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            SoundController.Instance.ReturnToPool(this);
        }

        public void Initialize(SoundData data) {
            Data = data;
            _audioSource.clip = data.clip;
            _audioSource.outputAudioMixerGroup = data.mixerGroup;
            _audioSource.volume = data.volume;
            _audioSource.loop = data.loop;
            _audioSource.playOnAwake = data.playOnAwake;
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) {
            _audioSource.pitch += Random.Range(min, max);
        }
    }
}
