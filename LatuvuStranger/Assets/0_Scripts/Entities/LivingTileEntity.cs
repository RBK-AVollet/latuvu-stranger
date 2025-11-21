using UnityEngine;

namespace Latuvu
{
    public class LivingTileEntity : TileEntity
    {
        public virtual void HandlePlayerOverlap(PlayerTileEntity player) 
        {
            Debug.Log("[LivingTileEntity]: Player overlapped with living tile entity at " + transform.position);
            player.KillSelf();
        }
        
        protected override void Tick()
        {
            Debug.Log("[LivingTileEntity]: Ticking living tile entity at " + transform.position);
        }
    }
}
