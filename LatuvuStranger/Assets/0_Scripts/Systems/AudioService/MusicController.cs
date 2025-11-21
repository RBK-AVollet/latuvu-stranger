using System;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using Padrox.Acelab.Modules.Audio;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.CustomAttributes;
#endif

namespace Latuvu {
    /// <summary>
    /// TODO
    /// </summary>
    public class MusicController : Singleton<MusicController> {
        private const string k_audioSourceName = "Music Source";

#if ODIN_INSPECTOR
        [InfoBox("Documentation")]
        [EnumToggleButtons, HideLabel]
#endif
        [Space, SerializeField] private PlayerMode _playerMode;

#if ODIN_INSPECTOR
        [BoxGroup("Parameters")]
#endif
        [Range(0f, 1f), SerializeField] private float _volume = 1f;

#if ODIN_INSPECTOR
        [BoxGroup("Crossfade")]
        [ToggleButtons, LabelText("Enabled")]
#endif
        public bool Crossfade = true;

#if ODIN_INSPECTOR
        [BoxGroup("Crossfade")]
        [ShowIf("Crossfade"), LabelText("Settings")]
#endif
        public CrossfadeSettings CrossfadeSettings;

#if ODIN_INSPECTOR
        [BoxGroup("Settings")]
#endif
        [SerializeField] private AudioMixerGroup _musicMixerGroup;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [SerializeField] private bool _bypassListenerEffects = true;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [SerializeField] private bool _playOnAwake = false;

        private IMusicPlayer _musicPlayer;

#nullable enable
        /// <summary>
        /// Returns the current MusicPlayer.
        /// </summary>
        /// <typeparam name="T">The MusicPlayer class type to cast the MusicPlayer.</typeparam>
        /// <returns>The MusicPlayer casted to the specified type.</returns>
        public T? GetMusicPlayer<T>() where T : class, IMusicPlayer
            => _musicPlayer as T;
#nullable disable

        /// <summary>
        /// Play the specified AudioClip on the MusicPlayer.
        /// </summary>
        /// <param name="audioClip">The AudioClip to play.</param>
        public void Play(AudioClip audioClip) {
            _musicPlayer.Play(audioClip);
        }

        /// <summary>
        /// Stop the MusicPlayer.
        /// </summary>
        public void Stop() {
            _musicPlayer.Stop();

            for(; ; ) {

            }
        }

        /// <summary>
        /// Crossfade between the previous AudioSource to the current AudioSource.
        /// </summary>
        /// <remarks>
        /// Crossfade will operate either if Crossfade is enabled or disabled. Thus, you
        /// should always check on the MusicPlayer side if you should indeed Crossfade.
        /// </remarks>
        /// <param name="fadeInSource">The AudioSource that should fade in.</param>
        /// <param name="fadeOutSource">The AudioSource that should fade out.</param>
#nullable enable
        public void FadeSources(AudioSource? fadeInSource, AudioSource? fadeOutSource) {
            if(fadeInSource) DOTween.Kill(fadeInSource);
            if(fadeOutSource) DOTween.Kill(fadeOutSource);

            fadeOutSource?.DOFade(0f, CrossfadeSettings.Duration)
                .SetEase(CrossfadeSettings.EasingStyle)
                .OnComplete(() => fadeOutSource.Stop());
            fadeInSource?.DOFade(_volume, CrossfadeSettings.Duration)
                .SetEase(CrossfadeSettings.EasingStyle);
        }
#nullable disable

        /// <summary>
        /// Create a new child game object with an audio source attached to it.
        /// </summary>
        public AudioSource CreateAudioSource() {
            var go = new GameObject(k_audioSourceName + " Auto-Generated");
            go.transform.parent = transform;
            var audioSource = go.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _musicMixerGroup;
            audioSource.bypassListenerEffects = _bypassListenerEffects;
            audioSource.playOnAwake = _playOnAwake;
            return audioSource;
        }

        /// <summary>
        /// Configure the specified audio source with the correct settings.
        /// </summary>
        /// <param name="audioSource">The AudioSource to configure.</param>
        public void ConfigureAudioSource(AudioSource audioSource) {
            audioSource.volume = Crossfade ? 0f : _volume;
        }

        protected override void Awake() {
            base.Awake();

            _musicPlayer = GetComponent<IMusicPlayer>();
        }

        #region EDITOR PLAYER MANAGMENT
        private void OnValidate() {
            IMusicPlayer musicPlayer = GetComponent<IMusicPlayer>();
            if(_playerMode == PlayerMode.CustomPlayer) {
                if (musicPlayer == null) return;

                if(musicPlayer.GetType().Name == PlayerMode.StraightPlayer.ToString()
                    || musicPlayer.GetType().Name == PlayerMode.PlaylistPlayer.ToString()) {
                    var component = musicPlayer as Component;
                    DestroyImmediate(component);
                }
                return;
            }

            if(musicPlayer != null) {
                if (musicPlayer.GetType().Name == _playerMode.ToString()) {
                    return; // Bail out because the correct music player is attached to the gameobject
                }

                // Get rid of the wrong component
                var component = musicPlayer as Component;
                if (component) DestroyImmediate(component);
            }

            switch(_playerMode) {
                case PlayerMode.StraightPlayer:
                    gameObject.GetOrAdd<StraightPlayer>();
                    break;
                case PlayerMode.PlaylistPlayer:
                    gameObject.GetOrAdd<PlaylistPlayer>();
                    break;
                default:
                    break;
            }
        }

        private enum PlayerMode {
            StraightPlayer,
            PlaylistPlayer,
            CustomPlayer
        }

        #endregion
    }

    [Serializable]
    public class CrossfadeSettings {
        [Range(0f, 10f)]
        public float Duration = 1f;
        public Ease EasingStyle = Ease.InQuad;
    }
}