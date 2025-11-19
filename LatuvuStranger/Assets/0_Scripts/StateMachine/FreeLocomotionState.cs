using UnityEngine;

namespace Latuvu
{
    public class FreeLocomotionState : BaseState
    {
        public FreeLocomotionState(PlayerController player, Animator animator) : base(player, animator)
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
            _player.HandleFreeMovement();
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