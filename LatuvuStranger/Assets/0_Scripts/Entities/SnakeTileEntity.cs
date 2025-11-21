using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class SnakeTileEntity : LivingTileEntity
    {
        protected override void Tick()
        {
            TryMoveOrFlip();
        }

        protected void TryMoveOrFlip()
        {
            FloorService floor = FloorService.Instance;
            Tilemap tilemap = floor.Tilemap;
            Vector3Int target = Position + Direction;
            
            GameTile currentTile = floor.Tilemap.GetTile<GameTile>(Position);
            GameTile targetTile = floor.Tilemap.GetTile<GameTile>(target);

            if (!targetTile)
            {
                Debug.Log("[SnakeTileEntity]: no tile at " + target);
                FlipDirection();
                return;
            }
            
            if (!targetTile.IsWalkable)
            {
                Debug.Log("[SnakeTileEntity]: tile not walkable at " + target);
                FlipDirection();
                return;
            }
            
            if (floor.TryGetEntityAtPos(target, out TileEntity staticEntity))
            {
                Debug.Log("[SnakeTileEntity]: trying to move static entity at " + target);
                FlipDirection();
                return;
            }
            
            currentTile.OnExit(floor.Tilemap, Position);
            
            Vector3 worldPos = tilemap.GetCellCenterWorld(target);
            transform.position = worldPos;

            Position = target;
            
            targetTile.OnEnter(tilemap, Position);
        }

        private void FlipDirection()
        {
            Direction = new Vector3Int(-Direction.x, -Direction.y, -Direction.z);
        }
    }
}
