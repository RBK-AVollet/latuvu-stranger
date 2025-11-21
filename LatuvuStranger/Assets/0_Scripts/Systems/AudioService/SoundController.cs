using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Padrox.Acelab.Modules.Audio;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.CustomAttributes;
#endif

namespace Latuvu {
    /// <summary>
    /// Credits: <see href="https://www.youtube.com/@git-amend">git-amend</see>
    /// </summary>
    public class SoundController : Singleton<SoundController> {
        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeSoundEmitters = new();
        public readonly Dictionary<SoundData, int> Counts = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

#if ODIN_INSPECTOR
        [Required, AssetsOnly]
#endif
        [SerializeField, Space]
        private SoundEmitter _soundEmitterPrefab;

#if ODIN_INSPECTOR
        [ToggleButtons]
#endif
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private int _maxSoundInstances = 30;

        protected override void Awake() {
            base.Awake();
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data) {
            if (!data.frequentSound) return true;

            if(FrequentSoundEmitters.Count >= _maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter)) {
                try {
                    soundEmitter.Stop();
                    return true;
                } catch {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }

            return true;
        }

        public SoundEmitter Get() {
            return _soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter) {
            _soundEmitterPool.Release(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter) {
            Destroy(soundEmitter.gameObject);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter) {
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter) {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        private SoundEmitter CreateSoundEmitter() {
            var soundEmitter = Instantiate(_soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void InitializePool() {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }
    }
}