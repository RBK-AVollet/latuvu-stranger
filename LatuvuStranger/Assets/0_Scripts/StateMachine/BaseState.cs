namespace Latuvu
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController _player;

        protected bool _debugMode = false;

		protected BaseState(PlayerController player)
        {
            _player = player;
        }
        
        public virtual void OnEnter()
        { }

        public virtual void Update()
        { }

        public virtual void FixedUpdate()
        { }

        public virtual void OnExit()
        { }
    }
}