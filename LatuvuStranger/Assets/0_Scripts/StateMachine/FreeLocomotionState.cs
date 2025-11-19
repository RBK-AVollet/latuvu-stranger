using UnityEngine;

namespace Latuvu
{
    public class FreeLocomotionState : BaseState
    {
        public FreeLocomotionState(PlayerController player) : base(player)
        { }

        public override void OnEnter()
        {
            Debug.Log("OnEnter FreeLocomotionState");
        }

        public override void FixedUpdate()
        {
            _player.HandleFreeMovement();
        }

        public override void OnExit()
        {
            Debug.Log("OnExit FreeLocomotionState");
        }
    }
}