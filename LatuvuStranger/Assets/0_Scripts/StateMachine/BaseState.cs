using UnityEngine;

namespace Latuvu
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerTileEntity _player;
        protected readonly Animator _animator;
        
        protected bool _debugMode = false;

		protected BaseState(PlayerTileEntity player, Animator animator)
        {
            _player = player;
            _animator = animator;
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