using Padrox.Acelab.Modules.Audio;
using UnityEngine;

namespace Latuvu
{
    /// <summary>
    /// Credits: <see href="https://www.youtube.com/@git-amend">git-amend</see>
    /// </summary>
    public class SoundBuilder {
        private readonly SoundController _audioController;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;

        public SoundBuilder(SoundController audioController) {
            _audioController = audioController;
        }

        public SoundBuilder WithSoundData(SoundData soundData) {
            _soundData = soundData;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position) {
            _position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch() {
            _randomPitch = true;
            return this;
        }

        public SoundEmitter Play() {
            if (!_audioController.CanPlaySound(_soundData)) return null;

            SoundEmitter soundEmitter = _audioController.Get();
            soundEmitter.Initialize(_soundData);
            soundEmitter.transform.position = _position;
            soundEmitter.transform.parent = _audioController.transform;

            if(_randomPitch) {
                soundEmitter.WithRandomPitch();
            }

            if(_soundData.frequentSound) {
                _audioController.FrequentSoundEmitters.Enqueue(soundEmitter);
            }

            soundEmitter.Play();
            return soundEmitter;
        }
    }
}
