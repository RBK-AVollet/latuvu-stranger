using UnityEngine;

namespace Latuvu
{
    public class SnakeTileEntity : LivingTileEntity
    {
        protected override void Tick()
        {
            TryMoveOrFlip();
        }
    }
}
