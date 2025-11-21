using UnityEngine;

namespace Latuvu
{
    public class LarvaeEntity : LivingTileEntity
    {
        protected override void Tick()
        {
            TryMoveOrFlip();
        }
    }
}
