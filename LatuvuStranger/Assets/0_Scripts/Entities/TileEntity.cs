using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class TileEntity : MonoBehaviour
    {
        [field:SerializeField] public Vector3Int Position { get; set; }
        [field:SerializeField] public Vector3Int Direction { get; set; }
        [field:SerializeField] public Sprite Skin { get; private set; }
        [field:SerializeField] public AffectDirection MoveDirections { get; private set; }
        [field:SerializeField] public AffectDirection InteractDirections { get; private set; }

        private void Start()
        {
            GameService.Instance.RegisterTickAction(Tick);
        }

        private void OnDestroy()
        {
            if (GameService.Instance != null)
            {
                GameService.Instance.UnregisterTickAction(Tick);
            }
        }
        
        public virtual void TryInteract(PlayerTileEntity player)
        {
            Debug.Log("Interacted with tile entity at position: " + Position);
        }

        protected virtual void Tick()
        { }

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
