using System;
using UnityEngine;
using UnityEngine.Audio;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.CustomAttributes;
#endif

namespace Padrox.Acelab.Modules.Audio {
    /// <summary>
    /// Credits: <see href="https://www.youtube.com/@git-amend">git-amend</see>
    /// </summary>
    [Serializable]
    public class SoundData {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        [Range(0f, 1f)] public float volume = 1f;

#if ODIN_INSPECTOR
        [ToggleButtons]
#endif
        public bool loop;
#if ODIN_INSPECTOR
        [ToggleButtons]
#endif
        public bool playOnAwake;
#if ODIN_INSPECTOR
        [ToggleButtons]
#endif
        public bool frequentSound;
    }
}
