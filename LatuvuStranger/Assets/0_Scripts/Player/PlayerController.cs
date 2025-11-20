using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Latuvu
{
    public class PlayerController : MonoBehaviour
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
             //   _playerInventory.RemoveCube();
            }

          //  GameService.Instance.Tick();
        }

        
        private void GridMoveStep(InputAction.CallbackContext context)
        {
            ResetVelocity();

            MoveDirection = context.ReadValue<Vector2>();
            
            Vector2 start = transform.position;
            Vector2 end = start + _tileStep * MoveDirection;
            
            PlayDirectionAnimation(MoveDirection);
            transform.position = end;
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

