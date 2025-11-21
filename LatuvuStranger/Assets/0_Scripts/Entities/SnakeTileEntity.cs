using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class SnakeTileEntity : LivingTileEntity
    {
        [field:SerializeField] public Sprite LeftSkin { get; private set; }
        [field:SerializeField] public Sprite RightSkin { get; private set; }
        private Sprite _currentSkin;
        private SpriteRenderer _skinRenderer;
        
        protected override void Start()
        {
            base.Start();

            _skinRenderer = GetComponent<SpriteRenderer>();
            UpdateSkin();
        }

        private void UpdateSkin()
        {
            if (Direction == Vector3Int.left) { _currentSkin = LeftSkin; }
            if (Direction == Vector3Int.right) { _currentSkin = RightSkin; }
            _skinRenderer.sprite = _currentSkin;
        }

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
                FlipDirection();
                return;
            }
            
            if (!targetTile.IsWalkable)
            {
                FlipDirection();
                return;
            }
            
            if (floor.TryGetEntityAtPos(target, out TileEntity entity))
            {
                FlipDirection();
                return;
            }
            
            if (FloorService.Instance.Player.Position == target)
            {
                FloorService.Instance.Player.KillSelf();
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
            UpdateSkin();
        }
    }
}
