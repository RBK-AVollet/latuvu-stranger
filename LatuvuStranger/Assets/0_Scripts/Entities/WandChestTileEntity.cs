using UnityEngine;

namespace Latuvu
{
    public class WandChestTileEntity : ChestTileEntity
    {
        protected override void OpenChest(PlayerTileEntity player)
        {
            base.OpenChest(player);

            GameUIMgr.Instance.ShowDialogue("You got THE WAND!");
            player.Inventory.ObtainWand();
        }
    }
}
