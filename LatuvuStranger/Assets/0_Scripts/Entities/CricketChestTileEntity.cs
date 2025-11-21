using UnityEngine;

namespace Latuvu
{
    public class CricketChestTileEntity : ChestTileEntity
    {
        protected override void OpenChest(PlayerTileEntity player)
        {
            base.OpenChest(player);
            
            player.Inventory.ObtainCrickets(1);
        }
    }
}
