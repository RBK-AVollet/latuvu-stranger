using UnityEngine;

namespace Latuvu
{
    public class DeathState : BaseState
    {
        public DeathState(PlayerTileEntity player, Animator animator) : base(player, animator)
        { }

        public override void OnEnter()
        {
            //_player.HandleDeath();
            _player.ResetVelocity();
        }
    }
}