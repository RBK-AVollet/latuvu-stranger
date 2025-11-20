using System;

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
            TickAction.Invoke();
        }
    }
}
