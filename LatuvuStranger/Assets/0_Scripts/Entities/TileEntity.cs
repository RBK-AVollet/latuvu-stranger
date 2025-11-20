using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class TileEntity : MonoBehaviour
    {
        [field:SerializeField] public Vector3Int Position { get; set; }
        [field:SerializeField] public Sprite Skin { get; private set; }
        [field:SerializeField] public AffectDirection MoveDirections { get; private set; }
        [field:SerializeField] public AffectDirection InteractDirections { get; private set; }

        private void Start()
        {
            GameService.Instance.RegisterTickAction(Tick);
        }

        public virtual bool TryMove(Vector3Int relativePosition, Tilemap tilemap)
        {
            Vector3Int targetCellPos = Position + relativePosition;
            
            GameTile targetTile = tilemap.GetTile<GameTile>(targetCellPos);

            if (!targetTile)
            {
                // TODO : Delete the entity and remove it from the floor service
                return true;
            }

            if (!targetTile.IsWalkable) return false;
            
            if (FloorService.Instance.TryGetEntityAtPos(targetCellPos, out TileEntity entity))
            {
                if (entity.TryGetComponent<LivingTileEntity>(out LivingTileEntity livingTileEntity))
                {   
                    // TODO : Stomp the living entity
                    return false;
                }
                return false;
            }
            
            Position = targetCellPos;
            return true;
        }
        
        public virtual void TryInteract(PlayerController player)
        {
            Debug.Log("Interacted with tile entity at position: " + Position);
        }

        protected virtual void Tick()
        {
            Vector3 pos = FloorService.Instance.Tilemap.GetCellCenterWorld(Position);
            transform.position = pos;
        }
    }
}
