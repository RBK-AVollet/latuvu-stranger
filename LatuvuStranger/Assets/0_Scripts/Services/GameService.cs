using System;
using UnityEngine;

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

        private void  Start()
        {
            
        }
    }
}
