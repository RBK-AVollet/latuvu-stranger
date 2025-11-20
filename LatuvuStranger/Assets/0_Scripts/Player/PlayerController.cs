using System;
using UnityEngine.InputSystem;
using UnityEngine;

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
        
        private bool _isAlive = true;
        public Vector2 MoveDirection { get; private set; }

        private void Awake()
        {
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
            _floorService = FloorService.Instance;
            
            transform.position = _floorService.Tilemap.GetCellCenterWorld(
                _floorService.Tilemap.WorldToCell(transform.position)
            );
            
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
                Mathf.RoundToInt(_tileStep * MoveDirection.x),
                Mathf.RoundToInt(_tileStep * MoveDirection.y),
                0
            );

            Vector3Int targetCell = currentCell + dir;

            Debug.Log($"Interact: checking cell {targetCell}");

            if (_floorService.TryGetEntityAtPos(targetCell, out TileEntity tileEntity))
            {
                // Noop
            }

            //  GameService.Instance.Tick();
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

            var tilemap = _floorService.Tilemap;

            Vector3Int currentCell = tilemap.WorldToCell(transform.position);

            Vector3Int targetCell = currentCell + new Vector3Int(
                (int)MoveDirection.x,
                (int)MoveDirection.y,
                0
            );

            if (!tilemap.HasTile(targetCell))
            {
                Debug.Log("[PlayerController]: no tile at " + targetCell);
                return;
            }

            Vector3 worldPos = tilemap.GetCellCenterWorld(targetCell);
            transform.position = worldPos;

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
    }
}

