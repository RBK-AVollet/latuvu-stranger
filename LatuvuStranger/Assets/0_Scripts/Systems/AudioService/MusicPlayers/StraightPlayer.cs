using UnityEngine;
using Latuvu;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.CustomAttributes;
#endif

namespace Padrox.Acelab.Modules.Audio
{
    [RequireComponent(typeof(MusicController))]
    public class StraightPlayer : MusicPlayerBase, IMusicPlayer {
#if ODIN_INSPECTOR
        [BoxGroup("DefaultMusic")]
        [ToggleButtons]
#endif
        [SerializeField] protected bool _useDefaultMusic = false;

#if ODIN_INSPECTOR
        [BoxGroup("DefaultMusic")]
        [ShowIf("_useDefaultMusic")]
        [HideLabel]
#endif
        [SerializeField] private AudioClip _defaultMusic;

#if ODIN_INSPECTOR
        [BoxGroup("Settings")]
        [ToggleButtons]
#endif
        [Tooltip("Should the playing track loop.")]
        [SerializeField] protected bool _loop;

        protected virtual void Start() {
            if (!_useDefaultMusic) return;

            Play(_defaultMusic);
        }

        protected virtual void Update() {
            if (_loop) {
                if (IsPaused) return;
                if (_musicController.Crossfade && !ShouldFadeOut()) return;
                if (!_musicController.Crossfade && _currentSource.isPlaying) return;

                Restart();
            } else {
                if (!_musicController.Crossfade || !ShouldFadeOut()) return;

                FadeOut();
            }
        }
    }
}
