using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Latuvu
{
    public class PlayerTileEntity : TileEntity
    {
        [SerializeField] private bool _debugMode;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private int _tileStep = 1;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _fallTimerDuration = 1f;
        [SerializeField] private float _speed = 5f;

        private Vector3Int _currentCell;
        private Tilemap _currentTilemap;

        private float _fallingTimeRemaining = 0f;
        private Vector3Int _fallOriginCell;
        private FloorService _floorService;

        private bool _hasFallOrigin = false;
        private bool _isFallTimerRunning;
        private float _oldAnimNormalizedTime = 0f;

        private int _oldAnimStateHash = 0;
        private Vector3Int _oldCell;
        private Vector2 _oldMoveDirection;
        private Vector3 _oldPosition;
        private Quaternion _oldRotation;

        private InputService _inputService;
        private PlayerInventory _playerInventory;

        private PlayerState _state = PlayerState.Normal;
        private MovementMode _movementMode = MovementMode.Free;

        public Vector2 MoveDirection { get; private set; }
        public PlayerInventory Inventory => _playerInventory;

        private enum PlayerState { Normal, Falling }
        private enum MovementMode { Free, Grid }

        private void Awake()
        {
            _floorService = FloorService.Instance;
            _playerInventory = new PlayerInventory();

            if (!_rb && _debugMode)
                _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _inputService = InputService.Instance;

            _inputService.RegisterWandInteraction(Interact);
            _inputService.RegisterTestAction(_playerInventory.ObtainWand);
        }

        private void FixedUpdate()
        {
            if (_movementMode == MovementMode.Free) 
                HandleFreeMovement();
        }

        private void Update()
        {
            if (!_isFallTimerRunning) return;

            _fallingTimeRemaining -= Time.deltaTime;

            Vector3Int currentCellNow = _floorService.Tilemap.WorldToCell(transform.position);

            if (_state == PlayerState.Falling && _hasFallOrigin && currentCellNow == _fallOriginCell)
            {
                transform.position = _oldPosition;
                transform.rotation = _oldRotation;
                MoveDirection = _oldMoveDirection;

                _currentCell = _oldCell;
                Position = _oldCell;

                if (_animator != null)
                {
                    _animator.ResetTrigger("Left");
                    _animator.ResetTrigger("Right");
                    _animator.ResetTrigger("Top");
                    _animator.ResetTrigger("Down");

                    _animator.Play(_oldAnimStateHash, 0, _oldAnimNormalizedTime);
                    _animator.Update(0);
                }

                ResetFallTimer();
                _hasFallOrigin = false;
                _state = PlayerState.Normal;

                return;
            }

            if (_fallingTimeRemaining > 0f)
                return;

            ResetFallTimer();
            OnFallTimerElapsed();
        }

        private void OnDestroy()
        {
            _inputService.UnregisterMoveAction(GridMoveStep);
            _inputService.UnregisterWandInteraction(Interact);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (_state == PlayerState.Falling) return;
            if (!_playerInventory.HasWand) return;

            var tilemap = _floorService.Tilemap;
            Vector3Int currentCell = tilemap.WorldToCell(transform.position);

            Vector3Int dir = new Vector3Int(
                Mathf.RoundToInt(MoveDirection.x),
                Mathf.RoundToInt(MoveDirection.y),
                0
            );

            if (dir == Vector3Int.zero)
                return;

            Vector3Int targetCell = currentCell + dir * _tileStep;
            GameTile tileInFront = tilemap.GetTile<GameTile>(targetCell);

            if (!_playerInventory.HasTile)
            {
                if (tileInFront != null && tileInFront.IsPickable)
                {
                    _playerInventory.StoreTile(tileInFront);
                    tileInFront.OnPickup(tilemap, targetCell);
                    tilemap.SetTile(targetCell, null);
                }
                return;
            }

            if (_playerInventory.HasTile)
            {
                if (tileInFront != null) return;

                GameTile tileToPlace = _playerInventory.Tile;
                tilemap.SetTile(targetCell, tileToPlace);
                _playerInventory.ClearTile();
            }
        }

        private void HandleFreeMovement()
        {
            if (_movementMode != MovementMode.Free) return;

            var moveInput = InputService.Instance.Player.ReadValue<Vector2>();
            _rb.linearVelocity = moveInput * _speed;
            
            var newCurrentCell = _floorService.Tilemap.WorldToCell(transform.position);
            if (newCurrentCell != _currentCell)
            {
                var previousCell = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
                previousCell?.OnExit(_floorService.Tilemap, _currentCell);
                
                _currentCell = newCurrentCell;
                Position = _currentCell;
                
                var currentCellTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
                
                if(currentCellTile) 
                    currentCellTile.OnEnter(_floorService.Tilemap, _currentCell);
                else 
                    KillSelf();
            }
            
            PlayDirectionAnimation(moveInput);
        }

        public void EnableGridMovement()
        {
            if (_movementMode == MovementMode.Grid) return;

            _movementMode = MovementMode.Grid;

            _inputService.RegisterMoveAction(GridMoveStep);

            ResetVelocity();

            _currentCell = _floorService.Tilemap.WorldToCell(transform.position);
        }

        private void GridMoveStep(InputAction.CallbackContext context)
        {
            if (_movementMode != MovementMode.Grid) return;

            Vector2 input = context.ReadValue<Vector2>();
            Vector2 rounded = new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));

            if (rounded == Vector2.zero)
                return;

            if (_state == PlayerState.Falling)
            {
                Vector3Int attemptedTarget = _currentCell + new Vector3Int((int)rounded.x, (int)rounded.y, 0);

                if (_hasFallOrigin && attemptedTarget == _fallOriginCell)
                {
                    transform.position = _oldPosition;
                    transform.rotation = _oldRotation;
                    MoveDirection = _oldMoveDirection;

                    _currentCell = _oldCell;
                    Position = _oldCell;

                    PlayDirectionAnimation(_oldMoveDirection);

                    ResetFallTimer();
                    _hasFallOrigin = false;
                    _state = PlayerState.Normal;

                    return;
                }
                else
                {
                    return;
                }
            }

            _oldPosition = transform.position;
            _oldRotation = transform.rotation;
            _oldMoveDirection = MoveDirection;
            _oldCell = _currentCell;

            if (_animator != null)
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);
                _oldAnimStateHash = info.fullPathHash;
                _oldAnimNormalizedTime = info.normalizedTime;
            }

            MoveDirection = rounded;
            PlayDirectionAnimation(MoveDirection);

            _currentTilemap = _floorService.Tilemap;

            Vector3Int targetCellPos = _currentCell + new Vector3Int((int)MoveDirection.x, (int)MoveDirection.y, 0);

            GameTile currentTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
            GameTile targetTile = _floorService.Tilemap.GetTile<GameTile>(targetCellPos);

            if (!targetTile)
            {
                _fallOriginCell = _currentCell;
                _hasFallOrigin = true;
                _state = PlayerState.Falling;

                StartFallingTimer();

                transform.position = _currentTilemap.GetCellCenterWorld(targetCellPos);

                _currentCell = targetCellPos;
                Position = targetCellPos;

                ResetVelocity();

                return;
            }

            if (targetTile && !targetTile.IsWalkable)
            {
                GameService.Instance.Tick();
                return;
            }

            if (_floorService.TryGetStaticEntityAtPos(targetCellPos, out StaticTileEntity staticEntity))
            {
                staticEntity.TryMove(GridHelper.GetRelativePosition(Position, staticEntity.Position), _currentTilemap);
                GameService.Instance.Tick();
                return;
            }

            Position = targetCellPos;

            if (currentTile != null)
                currentTile.OnExit(_floorService.Tilemap, _currentCell);

            Vector3 worldPosition = _currentTilemap.GetCellCenterWorld(targetCellPos);
            transform.position = worldPosition;

            _currentCell = targetCellPos;

            targetTile.OnEnter(_floorService.Tilemap, _currentCell);

            ResetVelocity();

            if (_floorService.TryGetLivingEntityAtPos(targetCellPos, out LivingTileEntity livingEntity))
            {
                livingEntity.HandlePlayerOverlap(this);
                return;
            }

            GameService.Instance.Tick();
        }

        public void KillSelf()
        {
            string floorId = FloorService.Instance.CurrentFloor.FloorId;
            FloorService.Instance.LoadFloor(floorId);
            _playerInventory.ClearTile();
            ResetFallTimer();
            _hasFallOrigin = false;
            _state = PlayerState.Normal;
        }

        public void ResetVelocity()
        {
            if (_rb != null)
                _rb.linearVelocity = Vector2.zero;
        }

        private void StartFallingTimer(float duration = -1f)
        {
            float used = duration > 0f ? duration : _fallTimerDuration;
            _fallingTimeRemaining = used;
            _isFallTimerRunning = true;
        }

        private void ResetFallTimer()
        {
            _isFallTimerRunning = false;
            _fallingTimeRemaining = 0f;
        }

        private void OnFallTimerElapsed()
        {
            Vector3Int topTarget = Position + Vector3Int.up;
            GameTile topTargetTile = _floorService.Tilemap.GetTile<GameTile>(topTarget);

            if (topTargetTile && _floorService.TryGetStaticEntityAtPos(topTarget, out StaticTileEntity topStaticEntity))
            {
                if (topStaticEntity is TeleportStatueTileEntity)
                {
                    _floorService.LoadNextFloor(Inventory.Crickets);
                    return;
                }
            }

            KillSelf();
            ResetFallTimer();
        }

        private void PlayDirectionAnimation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                _animator.SetTrigger(direction.x > 0 ? "Right" : "Left");
            else
                _animator.SetTrigger(direction.y > 0 ? "Top" : "Down");
        }

        public void Respawn()
        {
            transform.position = _floorService.CurrentFloor.GetPlayerSpawnWorld();
            _currentCell = _floorService.Tilemap.WorldToCell(transform.position);

            ResetFallTimer();
            _hasFallOrigin = false;
            _state = PlayerState.Normal;

            _playerInventory.ClearTile();
        }
    }
}
