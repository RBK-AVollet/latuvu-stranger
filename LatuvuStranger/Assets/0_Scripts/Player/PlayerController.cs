using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class PlayerController : LivingTileEntity
    {
        [SerializeField] private bool _debugMode;
        
        [SerializeField] private Rigidbody2D _rb;
        
        [SerializeField] private float _tileStep = 1f;
        
        [SerializeField] private Animator _animator;
        
        private PlayerInventory _playerInventory;
        
        private InputService _inputService;
        private FloorService _floorService;
        
        private Tilemap _currentTilemap;
        private Vector3Int _currentCell;
        public Vector2 MoveDirection { get; private set; }

        private void Awake()
        {
            // Accessible can spawned by the floor service itself
            _floorService = FloorService.Instance;
            
            if (!_rb && _debugMode)
            {
                Debug.Log("[Rigidbody2D] not assigned in PlayerController, trying to get it from GameObject.");
                _rb = GetComponent<Rigidbody2D>();
            }
            
            _playerInventory = new PlayerInventory();
        }
        
        private void Start()
        {
            _inputService = InputService.Instance;
            
            // Input Actions
            _inputService.RegisterMoveAction(GridMoveStep);
            _inputService.RegisterWandInteraction(Interact);
        }

        private void OnDestroy()
        {
            _inputService.UnregisterMoveAction(GridMoveStep);
            _inputService.UnregisterWandInteraction(Interact);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (!_playerInventory.HasWand) 
                return;

            var tilemap = _floorService.Tilemap;

            Vector3Int currentCell = tilemap.WorldToCell(transform.position);

            Vector3Int dir = new Vector3Int(
                Mathf.RoundToInt(MoveDirection.x),
                Mathf.RoundToInt(MoveDirection.y),
                0
            );

            if (dir == Vector3Int.zero)
                return;

            Vector3Int targetCell = currentCell + dir;

            GameTile tileInFront = tilemap.GetTile<GameTile>(targetCell);

            if (!_playerInventory.HasTile)
            {
                if (tileInFront != null && tileInFront.IsPickable)
                {
                    _playerInventory.StoreTile(tileInFront);

                    tileInFront.OnPickup(tilemap, targetCell);
                    
                    tilemap.SetTile(targetCell, null);

                    Debug.Log("Player picked up tile " + tileInFront.name);
                }
                else
                {
                    Debug.Log("No pickable tile in front");
                }

                return;
            }

            if (_playerInventory.HasTile)
            {
                if (tileInFront != null)
                {
                    Debug.Log("Can't place: a tile is already in front.");
                    return;
                }

                GameTile tileToPlace = _playerInventory.Tile;

                tilemap.SetTile(targetCell, tileToPlace);

                _playerInventory.ClearTile();

                Debug.Log("Placed tile: " + tileToPlace.name);
            }
        }
        
        private void GridMoveStep(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();

            MoveDirection = new Vector2(
                Mathf.Round(MoveDirection.x),
                Mathf.Round(MoveDirection.y)
            );

            if (MoveDirection == Vector2.zero)
                return;

            PlayDirectionAnimation(MoveDirection);

            _currentTilemap = _floorService.Tilemap;
            
            Vector3Int targetCellPos = _currentCell + new Vector3Int(
                (int)MoveDirection.x,
                (int)MoveDirection.y,
                0
            );
            
            var currentTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
            var targetTile = _floorService.Tilemap.GetTile<GameTile>(targetCellPos);
            
            if (!_currentTilemap.HasTile(targetCellPos) || !targetTile.IsWalkable)
            {
                Debug.Log("[PlayerController]: no tile at " + targetCellPos);
                return;
            }
            
            currentTile.OnExit(_floorService.Tilemap, _currentCell);
            
            Vector3 worldPos = _currentTilemap.GetCellCenterWorld(targetCellPos);
            transform.position = worldPos;
            
            _currentCell = targetCellPos;
            
            targetTile.OnEnter(_floorService.Tilemap, _currentCell);

            ResetVelocity();
        }
        
        public void ResetVelocity()
        {
            _rb.linearVelocity = Vector2.zero;
        }
        
        private void PlayDirectionAnimation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    _animator.SetTrigger("Right");
                else
                    _animator.SetTrigger("Left");
            }
            else
            {
                if (direction.y > 0)
                    _animator.SetTrigger("Top");
                else
                    _animator.SetTrigger("Down");
            }
        }

        public void Respawn()
        {
            transform.position = _floorService.CurrentFloor.GetPlayerSpawnWorld();
            _currentCell = _floorService.Tilemap.WorldToCell(transform.position);
        }
    }
}

