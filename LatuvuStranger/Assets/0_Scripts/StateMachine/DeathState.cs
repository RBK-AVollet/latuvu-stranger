namespace Latuvu
{
    public class DeathState : BaseState
    {
        public DeathState(PlayerController player) : base(player)
        { }

        public override void OnEnter()
        {
            //_player.HandleDeath();
            _player.ResetVelocity();
        }
    }
}