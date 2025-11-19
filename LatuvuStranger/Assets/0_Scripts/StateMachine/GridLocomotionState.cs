using UnityEngine;

namespace Latuvu
{
    public class GridLocomotionState : BaseState
    {
        public GridLocomotionState(PlayerController player) : base(player)
        { }

        public override void OnEnter()
        {
            if (_debugMode)
            { 
                Debug.Log("OnEnter GridLocomotionState");
            }
            
            _player.ResetVelocity();
        }

        public override void FixedUpdate()
        {
            _player.HandleGridMovement();
        }

        public override void OnExit()
        {
            if (_debugMode)
            { 
                Debug.Log("OnExit GridLocomotionState");
            }
        }
    }
}