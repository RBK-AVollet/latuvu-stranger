using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Latuvu;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.CustomAttributes;
using Sirenix.OdinInspector;
#endif

namespace Padrox.Acelab.Modules.Audio {
    /// <summary>
    /// This is a solid and customizable implementation of a playlist music system.
    /// Credits: <see href="https://www.youtube.com/@git-amend">git-amend</see>
    /// </summary>
    /// <remarks>
    /// It is useful for any type of game where you want different tracks to play.
    /// </remarks>
    [RequireComponent(typeof(MusicController))]
    public class PlaylistPlayer : MusicPlayerBase, IMusicPlayer {
#if ODIN_INSPECTOR
        [BoxGroup("Playlist")]
        [ValidateInput("AllTracksLastEnough", "All tracks must last more than the FadeSettings.Duration")]
#endif
        [SerializeField] protected List<AudioClip> _initialPlaylist;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [Tooltip("Should the playlist loop ?")]
        [SerializeField] protected bool _loop = true;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [Tooltip("Determine if the music player will stop if the PlayNextTrack method is called on an empty playlist or continue playing the current track.")]
        [SerializeField] protected bool _playNextTrackStopsIfEmptyPlaylist = false;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [Tooltip("Shuffle the entire playlist everytime PlayNextTrack is called.")]
        [SerializeField] protected bool _shuffle;

#if ODIN_INSPECTOR
        [BoxGroup("Settings"), ToggleButtons]
#endif
        [Tooltip("If enabled, the Player will make sure the tracks in the playlist are longer than the fade duration.")]
        [SerializeField] protected bool _checkTrackLength = false;

        protected readonly Queue<AudioClip> _playlist = new();

        protected bool _playlistDirty = true;
        protected ReadOnlyCollection<AudioClip> _readOnlyPlaylist;
        public ReadOnlyCollection<AudioClip> Playlist { get {
                if (_playlistDirty) {
                    _playlistDirty = false;
                    _readOnlyPlaylist = new ReadOnlyCollection<AudioClip>(_playlist.ToList());
                }
                return _readOnlyPlaylist;
            }
        }

        /// <summary>
        /// Play the next track in the playlist if there is one to play.
        /// </summary>
        /// <remarks>
        /// Use _playNextTrackStopsIfEmptyPlaylist parameter to control if the
        /// AudioPlayer should stop if called on an empty playlist.
        /// </remarks>
        /// <returns>true if the next track started playing and false otherwise.</returns>
#if ODIN_INSPECTOR
        [PropertyOrder(-10)]
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.SkipForward, "", DrawResult = false)]
        [Tooltip("Play the next track in the playlist.")]
#endif
        public virtual bool PlayNextTrack() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return false;
#endif
            if (_playlist.TryDequeue(out AudioClip nextTrack)) {
                Play(nextTrack);
                if (_loop) {
                    AddToPlaylist(nextTrack);
                }
                _playlistDirty = true;
                return true;
            }

            if (!_playNextTrackStopsIfEmptyPlaylist) return false;
            if (_musicController.Crossfade) {
                FadeOut();
            } else {
                Stop();
            }

            return false;
        }

        /// <summary>
        /// Add the specified AudioClip to the playlist.
        /// </summary>
        /// <param name="clip">The AudioClip to add in the playlist.</param>
        public virtual void AddToPlaylist(AudioClip clip) {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (!clip) return;

            // Make sure the added track is longer than the fade duration.
            // Can be ignore by using the _checkTrackLength boolean field.
            if(_checkTrackLength && _musicController.Crossfade) {
                float fadeDuration = _musicController.CrossfadeSettings.Duration;
                if (!clip.LastMoreThan(fadeDuration)) {
                    Debug.Log("[MusicPlayer]: You are trying to add an audio clip to the playlist which last less than the fade duration !");
                    return;
                }
            }

            _playlist.Enqueue(clip);
            if (_shuffle) {
                ShufflePlaylist();
            }
            _playlistDirty = true;
        }

        /// <summary>
        /// Shuffle the entire playlist.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.Shuffle, "")]
        [Tooltip("Shuffle the playlist.")]
#endif
        public virtual void ShufflePlaylist() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            _playlist.Shuffle();
            _playlistDirty = true;
        }

        /// <summary>
        /// Clear the entire playlist.
        /// </summary>
#if ODIN_INSPECTOR
        [ButtonGroup("_ControlPlayer", ButtonHeight = 30), Button(SdfIconType.Trash, "")]
        [Tooltip("Clear the playlist.")]
#endif
        public virtual void Clear() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            _playlist.Clear();
            _playlistDirty = true;
        }

        /// <summary>
        /// Is the playlist empty right now ? (Read-Only)
        /// </summary>
        public virtual bool IsPlaylistEmpty() => _playlist.Count <= 0;

        /// <summary>
        /// Set if the playlist should loop or not.
        /// </summary>
        public virtual void SetLoop(bool loop) => _loop = loop;

        /// <summary>
        /// Set if the playlist should shuffle or not.
        /// </summary>
        public virtual void SetShuffle(bool shuffle) => _shuffle = shuffle;

        protected virtual void Update() {
#if UNITY_EDITOR
            // Simple fix for alt tabbing playing the next track in the Editor
            if (!Application.isFocused) return;
#endif
            if (IsPaused) return;
            if (_musicController.Crossfade && !ShouldFadeOut()) return;
            if (!_musicController.Crossfade && _currentSource.isPlaying) return;
            if (IsPlaylistEmpty()) return;

            PlayNextTrack();
        }

        protected virtual void Start() {
            InitializePlaylist();
        }

        /// <summary>
        /// Populate the playlist with all the AudioClips in the _initialPlaylist.
        /// </summary>
        protected virtual void InitializePlaylist() {
            foreach(var clip in  _initialPlaylist) {
                AddToPlaylist(clip);
            }
        }

#if ODIN_INSPECTOR
        protected virtual bool AllTracksLastEnough() {
            if (!_checkTrackLength) return true;
            MusicController musicController = GetComponent<MusicController>();
            if (!musicController.Crossfade) return true;
            if (_initialPlaylist == null) return true;
            float fadeDuration = musicController.CrossfadeSettings.Duration;
            return _initialPlaylist.All(t => t.LastMoreThan(fadeDuration));
        }
#endif
    }
}