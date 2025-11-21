using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Latuvu
{
    public class GameService : Singleton<GameService>
    {
        
        private event Action TickAction = delegate { };

        public void RegisterTickAction(Action callback)
        {
            TickAction += callback;
        }

        public void UnregisterTickAction(Action callback)
        {
            TickAction -= callback;
        }

        public void Tick()
        {
            //Debug.Log("[GameService]: Ticking the game");
            TickAction.Invoke();
        }

        // HOTFIX
        private void Update()
        {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                ES3.DeleteFile();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
