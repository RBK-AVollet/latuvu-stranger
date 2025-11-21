using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class StaticTileEntity : TileEntity
    {
        public virtual bool TryMove(Vector3Int relativePosition, Tilemap tilemap)
        {
            Vector3Int moveDir = Vector3Int.zero;
            if (relativePosition.x != 0 && relativePosition.y == 0)
                moveDir = (relativePosition.x > 0) ? Vector3Int.right : Vector3Int.left;
            else if (relativePosition.y != 0 && relativePosition.x == 0)
                moveDir = (relativePosition.y > 0) ? Vector3Int.up : Vector3Int.down;
            else
                return false;

            if (!MoveDirections.AllowsMovement(moveDir)) return false;

            Vector3Int targetCellPos = Position + relativePosition;
            
            GameTile targetTile = tilemap.GetTile<GameTile>(targetCellPos);

            if (!targetTile)
            {
                FloorService.Instance.RemoveEntity(this);
                Destroy(gameObject);
                return true;
            }

            if (!targetTile.IsWalkable) return false;
            
            if (FloorService.Instance.TryGetEntityAtPos(targetCellPos, out TileEntity entity))
            {
                if (entity.TryGetComponent<LivingTileEntity>(out LivingTileEntity livingTileEntity))
                {   
                    FloorService.Instance.RemoveEntity(livingTileEntity);
                    Destroy(livingTileEntity.gameObject);
                }
                else
                {
                    return false;
                }
            }
            
            Position = targetCellPos;
            
            Vector3 pos = FloorService.Instance.Tilemap.GetCellCenterWorld(Position);
            transform.position = pos;
            
            return true;
        }
    }
}
