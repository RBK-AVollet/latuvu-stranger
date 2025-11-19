namespace Latuvu
{
    public class GridLocomotionState : BaseState
    {
        public GridLocomotionState(PlayerController player) : base(player)
        { }

        public override void Update()
        {
            // Noop    
        }
        
        public override void FixedUpdate()
        {
            _player.HandleGridMovement();
        }
    }
}