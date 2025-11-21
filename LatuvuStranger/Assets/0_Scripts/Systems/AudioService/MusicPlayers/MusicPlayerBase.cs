using Latuvu;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.CustomAttributes;
#endif

namespace Padrox.Acelab.Modules.Audio {
    public abstract class MusicPlayerBase : MonoBehaviour, IMusicPlayer {
#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
        [InfoBox("Recommended when using Playlist Mode.", "IsPlaylistAndDisabled")]
        [PropertyOrder(15)]
        [ShowIf("IsCrossfadeEnabled")]
#endif
        [Tooltip("Used to determine if the GetTrackProgress returns 1 at the end of the track or at the fade timestamp, thus, when fading out. Recommended when using playlist mode.")]
        [SerializeField]
        protected bool _useFadeTimestampAsProgress = true;

        protected MusicController _musicController;

        protected AudioSource _currentSource;
        protected AudioSource _previousSource;

        public bool IsPaused { get; private set; }

        public bool IsPlaying => _currentSource ? _currentSource.isPlaying : false;

#nullable enable
        public AudioClip? GetPlayingAudioClip() => _currentSource ? _currentSource.clip : null;
#nullable disable

        /// <summary>
        /// Get the current track progression.
        /// </summary>
        /// <remarks>
        /// If no track is currently playing, always return 0.
        /// Take advantage of _useFadeTimstampAsProgress to control if one is the end of the track
        /// or the fade timestamp, thus, when fading out.
        /// </remarks>
        /// <returns>
        /// The progression of the current playing track.
        /// Zero being the start of the track and one being the end of the track.
        /// </returns>
        public float GetTrackProgress() {
            if (!_currentSource.isPlaying) return 0f;
            float length = _musicController.Crossfade && _useFadeTimestampAsProgress ? GetFadeTimestamp() : _currentSource.clip.length;
            return Mathf.Clamp01(_currentSource.time / length);
        }

        /// <summary>
        /// Play the specified AudioClip. 
        /// </summary>
        /// <param name="audioClip">The AudioClip to play.</param>
        public virtual void Play(AudioClip audioClip) {
            if (!audioClip) {
                Debug.LogWarning("[MusicPlayer]: You tried to play an AudioClip without providing it !");
                return;
            }

            // Use the previous as the current and use the current as the previous
            SwapAudioSources();

            _musicController.ConfigureAudioSource(_currentSource);
            _currentSource.clip = audioClip;
            _currentSource.Play();

            if(_musicController.Crossfade) {
                Crossfade();
            } else {
                _previousSource.Stop();
            }
        }

        /// <summary>
        /// Stop both the current and the previous AudioSources.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.Stop, "")]
        [Tooltip("Stop the music player.")]
#endif
        public virtual void Stop() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            _currentSource.Stop();
            _previousSource.Stop();
        }

        /// <summary>
        /// Pause both the current and the previous AudioSources.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.Pause, "")]
        [Tooltip("Pause the music player.")]
#endif
        public virtual void Pause() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            IsPaused = true;
            _currentSource.Pause();
            _previousSource.Pause();
        }

        /// <summary>
        /// Resume both the current and the previous AudioSources.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.Play, "")]
        [Tooltip("Resume the music player.")]
#endif
        public virtual void Resume() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            IsPaused = false;
            _currentSource.UnPause();
            _previousSource.UnPause();
        }

        /// <summary>
        /// Restart the current audioclip.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.ArrowClockwise, "")]
        [Tooltip("Resume the music player.")]
#endif
        public virtual void Restart() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            Play(_currentSource.clip);
        }

        /// <summary>
        /// Swap the current AudioSource to be the previous AudioSource and vice versa.
        /// </summary>
        protected void SwapAudioSources() {
            AudioSource tmp = _currentSource;
            _currentSource = _previousSource;
            _previousSource = tmp;
        }

        /// <summary>
        /// Fade in the current AudioSource.
        /// </summary>
        /// <remarks>
        /// Expect the AudioSource volume to be set to zero.
        /// </remarks>
        protected void FadeIn() => _musicController.FadeSources(_currentSource, null);
        
        /// <summary>
        /// Fade out the current AudioSource.
        /// </summary>
        protected void FadeOut() => _musicController.FadeSources(null, _currentSource);
        
        /// <summary>
        /// Crossfade the current AudioSource and the previous AudioSource.
        /// </summary>
        protected void Crossfade() => _musicController.FadeSources(_currentSource, _previousSource);

        /// <summary>
        /// Get the current playing AudioClip timestamp when we should initiate the fade.
        /// </summary>
        /// <remarks>
        /// This is only relevant when fade is enabled.
        /// </remarks>
        /// <returns>The timestamp at which we should start fading.</returns>
        protected float GetFadeTimestamp() {
            AudioClip currentClip = GetPlayingAudioClip();
            if (!currentClip) return 0f;

            float fadeDuration = _musicController.CrossfadeSettings.Duration;
            return currentClip.length - fadeDuration;
        }

        /// <summary>
        /// Determine if the clip is near enough the end to fade out.
        /// </summary>
        /// <returns>true if the MusicPlayer should fade out.</returns>
        protected virtual bool ShouldFadeOut()
            => !_currentSource.clip || _currentSource.time >= GetFadeTimestamp();

        protected virtual void Awake() {
            _musicController = gameObject.GetOrAdd<MusicController>();

            _currentSource = _musicController.CreateAudioSource();
            _previousSource = _musicController.CreateAudioSource();

            _currentSource.loop = false;
            _previousSource.loop = false;
        }

#if UNITY_EDITOR && ODIN_INSPECTOR
        protected virtual bool IsPlaylistAndDisabled() {
            return !_useFadeTimestampAsProgress && this is PlaylistPlayer playlist;
        }

        protected virtual bool IsCrossfadeEnabled() {
            MusicController controller = GetComponent<MusicController>();
            return controller.Crossfade;
        }
#endif
    }
}