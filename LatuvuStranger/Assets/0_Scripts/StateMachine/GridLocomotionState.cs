using UnityEngine;

namespace Latuvu
{
    public class GridLocomotionState : BaseState
    {
        public GridLocomotionState(PlayerTileEntity player, Animator animator) : base(player, animator)
        { }

        public override void OnEnter()
        {
            if (_debugMode)
            { 
                Debug.Log("OnEnter GridLocomotionState");
            }
            _player.ResetVelocity();
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