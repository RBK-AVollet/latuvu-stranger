using UnityEngine;

namespace Padrox.Acelab.Modules.Audio {
    public interface IMusicPlayer {
        void Play(AudioClip audioClip);
        void Stop();
        void Pause();
        void Resume();
        void Restart();
    }
}