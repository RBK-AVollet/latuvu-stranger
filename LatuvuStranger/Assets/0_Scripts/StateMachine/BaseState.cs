using UnityEngine;

namespace Latuvu
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController _player;
        protected readonly Animator _animator;
        
        protected static readonly int FreeLocomotionHash = Animator.StringToHash("FreeLocomotion");
        protected static readonly int GridLocomotionHash = Animator.StringToHash("GridLocomotion");
        protected static readonly int DeathHash = Animator.StringToHash("Death");
        
        protected const float _crossFadeDuration = 0.1f;

        protected bool _debugMode = false;

		protected BaseState(PlayerController player, Animator animator)
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