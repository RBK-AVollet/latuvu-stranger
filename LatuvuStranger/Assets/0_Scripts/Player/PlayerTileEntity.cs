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
        private Vector3Int _currentCell;

        private Tilemap _currentTilemap;

        private float _fallingTimeRemaining = 0f;

        private Vector3Int _fallOriginCell;
        private FloorService _floorService;
        private bool _hasFallOrigin = false;

        private InputService _inputService;
        private bool _isFallTimerRunning;
        private float _oldAnimNormalizedTime = 0f;

        private int _oldAnimStateHash = 0;
        private Vector3Int _oldCell;
        private Vector2 _oldMoveDirection;

        private Vector3 _oldPosition;
        private Quaternion _oldRotation;

        private PlayerInventory _playerInventory;
        private PlayerState _state = PlayerState.Normal;
        public Vector2 MoveDirection { get; private set; }

        public PlayerInventory Inventory => _playerInventory;
        
        private void Awake()
        {
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

            _inputService.RegisterMoveAction(GridMoveStep);
            _inputService.RegisterWandInteraction(Interact);
        }

        private void Update()
        {
            if (!_isFallTimerRunning) return;

            _fallingTimeRemaining -= Time.deltaTime;

            Vector3Int currentCellNow = _floorService.Tilemap.WorldToCell(transform.position);

            if (_state == PlayerState.Falling && _hasFallOrigin && currentCellNow == _fallOriginCell)
            {
                if (_debugMode) Debug.Log("[PlayerTileEntity] Returned to origin cell - performing rollback and cancelling fall.");

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
            OnQuickTimerElapsed();
        }

        private void OnDestroy()
        {
            _inputService.UnregisterMoveAction(GridMoveStep);
            _inputService.UnregisterWandInteraction(Interact);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (_state == PlayerState.Falling)
            {
                if (_debugMode) Debug.Log("[PlayerTileEntity] Interact blocked while falling.");
                return;
            }

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

            Vector3Int targetCell = currentCell + dir * _tileStep;
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
            Vector2 input = context.ReadValue<Vector2>();
            Vector2 rounded = new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));

            if (rounded == Vector2.zero)
                return;

            if (_state == PlayerState.Falling)
            {
                Vector3Int attemptedTarget = _currentCell + new Vector3Int((int)rounded.x, (int)rounded.y, 0);

                if (_hasFallOrigin && attemptedTarget == _fallOriginCell)
                {
                    if (_debugMode) Debug.Log("[PlayerTileEntity] Player attempts to move back to origin -> rollback.");

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
                    if (_debugMode) Debug.Log("[PlayerTileEntity] Move blocked while falling. Only move allowed is returning to origin cell.");
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
                if (_debugMode) Debug.Log("[PlayerController]: stepping into void at " + targetCellPos);

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
                Debug.Log("[PlayerController]: tile not walkable at " + targetCellPos);
                GameService.Instance.Tick();
                return;
            }

            if (_floorService.TryGetStaticEntityAtPos(targetCellPos, out StaticTileEntity staticEntity))
            {
                Debug.Log("[PlayerController]: trying to move static entity at " + targetCellPos);
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
                Debug.Log("[PlayerController]: moved onto living entity at " + targetCellPos);
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
            if (_debugMode) Debug.Log("[PlayerTileEntity] StartFallingTimer");
            float used = duration > 0f ? duration : _fallTimerDuration;
            _fallingTimeRemaining = used;
            _isFallTimerRunning = true;
        }

        private void ResetFallTimer()
        {
            _isFallTimerRunning = false;
            _fallingTimeRemaining = 0f;
        }

        private void OnQuickTimerElapsed()
        {
            if (_debugMode) Debug.Log($"[PlayerTileEntity] Quick timer elapsed");
            KillSelf();
            ResetFallTimer();
        }

        private void PlayDirectionAnimation(Vector2 direction)
        {
            if (direction == Vector2.zero) return;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                _animator.SetTrigger(direction.x > 0 ? "Right" : "Left");
            }
            else
            {
                _animator.SetTrigger(direction.y > 0 ? "Top" : "Down");
            }
        }

        public void Respawn()
        {
            transform.position = _floorService.CurrentFloor.GetPlayerSpawnWorld();
            _currentCell = _floorService.Tilemap.WorldToCell(transform.position);

            ResetFallTimer();
            _hasFallOrigin = false;
            _state = PlayerState.Normal;
        }

        private enum PlayerState { Normal, Falling }
    }
}
