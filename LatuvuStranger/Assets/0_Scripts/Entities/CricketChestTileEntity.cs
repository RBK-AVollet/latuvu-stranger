using UnityEngine;

namespace Latuvu
{
    public class CricketChestTileEntity : ChestTileEntity
    {
        protected override void OpenChest(PlayerTileEntity player)
        {
            base.OpenChest(player);
            
            GameUIMgr.Instance.ShowDialogue("You got 1 cricket!");
            player.Inventory.ObtainCrickets(1);
        }
    }
}
