using UnityEngine;

namespace Latuvu
{
    public class FreeLocomotionState : BaseState
    {
        public FreeLocomotionState(PlayerTileEntity player, Animator animator) : base(player, animator)
        { }

        public override void OnEnter()
        {
            if (_debugMode)
            { 
                Debug.Log("OnEnter FreeLocomotionState");
            }
            _player.ResetVelocity();
        }

        public override void FixedUpdate()
        {
        }

        public override void OnExit()
        {
            if (_debugMode)
            { 
                Debug.Log("OnExit FreeLocomotionState");
            }
        }
    }
}