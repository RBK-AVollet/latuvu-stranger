using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Latuvu
{
    public class PlayerTileEntity : TileEntity
    {
        [SerializeField] private bool _debugMode;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private int _tileStep = 1;
        [SerializeField] private float _fallTimerDuration = 1f;
        [SerializeField] private float _speed = 5f;

        private Vector3Int _currentCell;
        private Tilemap _currentTilemap;
        private FloorService _floorService;
        private InputService _inputService;
        private PlayerInventory _playerInventory;

        private float _fallingTimeRemaining;
        private bool _isFallTimerRunning;
        private bool _hasFallOrigin;
        private Vector3Int _fallOriginCell;

        private Vector3 _oldPosition;
        private Quaternion _oldRotation;
        private Vector3Int _oldCell;
        private Vector2 _oldMoveDirection;
        private int _oldAnimStateHash;
        private float _oldAnimNormalizedTime;

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

            if (!_rb)
                _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _inputService = InputService.Instance;
            _inputService.RegisterWandInteraction(Interact);
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
            var currentCellNow = _floorService.Tilemap.WorldToCell(transform.position);

            if (_state == PlayerState.Falling && _hasFallOrigin && currentCellNow == _fallOriginCell)
            {
                RollbackPosition();
                return;
            }

            if (_fallingTimeRemaining <= 0f)
            {
                ResetFallTimer();
                OnFallTimerElapsed();
            }
        }

        private void OnDestroy()
        {
            _inputService.UnregisterMoveAction(GridMoveStep);
            _inputService.UnregisterWandInteraction(Interact);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (_state == PlayerState.Falling || !_playerInventory.HasWand)
                return;

            var tilemap = _floorService.Tilemap;
            var currentCell = tilemap.WorldToCell(transform.position);

            var dir = new Vector3Int(
                Mathf.RoundToInt(MoveDirection.x),
                Mathf.RoundToInt(MoveDirection.y),
                0
            );

            if (dir == Vector3Int.zero) return;

            Vector3Int targetCell = currentCell + dir * _tileStep;
            
            GameTile tileInFront = tilemap.GetTile<GameTile>(targetCell);
            
            if (tileInFront && _floorService.TryGetEntityAtPos(targetCell, out TileEntity entityInFront))
            {
                entityInFront.TryInteract(GridHelper.GetRelativePosition(Position, entityInFront.Position), _currentTilemap, this);
                GameService.Instance.Tick();
                return;
            }

            if (!_playerInventory.HasTile)
            {
                if (tileInFront != null && tileInFront.IsPickable)
                {
                    _playerInventory.StoreTile(tileInFront);
                    tileInFront.OnPickup(tilemap, targetCell);
                    tilemap.SetTile(targetCell, null);
                    GameService.Instance.Tick();
                }
            }
            else if (_playerInventory.HasTile && tileInFront == null)
            {
                tilemap.SetTile(targetCell, _playerInventory.Tile);
                _playerInventory.ClearTile();
                Debug.Log("Placed tile: " + tileToPlace.name);
            }
        }

        private void HandleFreeMovement()
        {
            var moveInput = InputService.Instance.Player.ReadValue<Vector2>();
            _rb.linearVelocity = moveInput * _speed;

            var newCell = _floorService.Tilemap.WorldToCell(transform.position);
            if (newCell != _currentCell)
                UpdateCell(newCell);

            PlayDirectionAnimation(moveInput);
        }

        private void UpdateCell(Vector3Int newCell)
        {
            var previousTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
            previousTile?.OnExit(_floorService.Tilemap, _currentCell);

            _currentCell = newCell;
            Position = _currentCell;

            var currentTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
            if (currentTile != null)
                currentTile.OnEnter(_floorService.Tilemap, _currentCell);
            else
                KillSelf();
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

            var input = context.ReadValue<Vector2>();
            var rounded = new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));
            if (rounded == Vector2.zero) return;

            if (_state == PlayerState.Falling)
            {
                var attemptedTarget = _currentCell + new Vector3Int((int)rounded.x, (int)rounded.y, 0);
                if (_hasFallOrigin && attemptedTarget == _fallOriginCell)
                {
                    RollbackPosition();
                    return;
                }
                return;
            }

            SaveOldState();
            MoveDirection = rounded;
            PlayDirectionAnimation(MoveDirection);

            _currentTilemap = _floorService.Tilemap;
            var targetCellPos = _currentCell + new Vector3Int((int)MoveDirection.x, (int)MoveDirection.y, 0);

            var currentTile = _floorService.Tilemap.GetTile<GameTile>(_currentCell);
            var targetTile = _floorService.Tilemap.GetTile<GameTile>(targetCellPos);

            if (targetTile == null)
            {
                FallToVoid(targetCellPos);
                return;
            }

            if (!targetTile.IsWalkable)
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

            MoveToCell(targetCellPos, currentTile, targetTile);
        }

        private void SaveOldState()
        {
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
        }

        private void RollbackPosition()
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
        }

        private void FallToVoid(Vector3Int targetCellPos)
        {
            _fallOriginCell = _currentCell;
            _hasFallOrigin = true;
            _state = PlayerState.Falling;

            StartFallingTimer();
            transform.position = _currentTilemap.GetCellCenterWorld(targetCellPos);
            _currentCell = targetCellPos;
            Position = targetCellPos;
            ResetVelocity();
        }

        private void MoveToCell(Vector3Int targetCellPos, GameTile currentTile, GameTile targetTile)
        {
            Position = targetCellPos;

            currentTile?.OnExit(_floorService.Tilemap, _currentCell);

            transform.position = _currentTilemap.GetCellCenterWorld(targetCellPos);
            _currentCell = targetCellPos;
            targetTile?.OnEnter(_floorService.Tilemap, _currentCell);

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
            _animator.SetTrigger("Down");
            FloorService.Instance.LoadFloor(FloorService.Instance.CurrentFloor.FloorId);
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
            _fallingTimeRemaining = duration > 0 ? duration : _fallTimerDuration;
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
            if (_floorService.Tilemap.GetTile<GameTile>(topTarget) is GameTile topTile &&
                _floorService.TryGetStaticEntityAtPos(topTarget, out StaticTileEntity topStatic) &&
                topStatic is TeleportStatueTileEntity)
            {
                _floorService.LoadNextFloor(Inventory.Crickets);
                return;
            }

            KillSelf();
            ResetFallTimer();
        }

        private void PlayDirectionAnimation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;
            _animator.SetTrigger(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)
                ? (direction.x > 0 ? "Right" : "Left")
                : (direction.y > 0 ? "Top" : "Down"));
        }

        public void Respawn()
        {
            transform.position = _floorService.CurrentFloor.GetPlayerSpawnWorld();
            _currentCell = _floorService.Tilemap.WorldToCell(transform.position);
            ResetFallTimer();
            _hasFallOrigin = false;
            _state = PlayerState.Normal;
            _animator.SetTrigger("Down");
            _playerInventory.ClearTile();
        }
    }
}
