using UnityEngine;

namespace Latuvu
{
    public class PlayerTileEntity : LivingTileEntity
    {
        [SerializeField] private PlayerInventory _inventory;
        [SerializeField] private PlayerController _playerController;

        protected override void Tick()
        {
            base.Tick();

            if (!_inventory.HasWand) return;
            
            
        }
    }
}
