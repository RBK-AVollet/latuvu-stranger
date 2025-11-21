using UnityEngine;

namespace Latuvu
{
    public class WandChestTileEntity : ChestTileEntity
    {
        protected override void OpenChest(PlayerTileEntity player)
        {
            base.OpenChest(player);

            player.Inventory.ObtainWand();
        }
    }
}
