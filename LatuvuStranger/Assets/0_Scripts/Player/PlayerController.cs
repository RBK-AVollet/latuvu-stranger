using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

namespace Latuvu
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private bool _debugMode;
        
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Rigidbody2D _rb;
        
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _tileStep = 1f;
        
        [SerializeField] private Animator _animator;
        
        private InputAction _moveAction;
        private InputAction _wandInteractionAction;
        
        private StateMachine _stateMachine;
        
        private PlayerInventory _playerInventory;
        
        private bool _isMovingGrid = false; 
        
        private bool _isAlive = true;

        private void Awake()
        {
            if (!_rb && _debugMode)
            {
                Debug.Log("[Rigidbody2D] not assigned in PlayerController, trying to get it from GameObject.");
                _rb = GetComponent<Rigidbody2D>();
            }
            if (!_playerInput && _debugMode)
            {
                Debug.Log("[PlayerInput] not assigned in PlayerController, trying to get it from GameObject.");
                _playerInput = GetComponent<PlayerInput>();
            }
            
            _playerInventory = new PlayerInventory();
            
            // State Machine
            _stateMachine = new StateMachine();
            
            // Declare States
            var freeLocomotionState = new FreeLocomotionState(this, _animator);
            var gridLocomotionState = new GridLocomotionState(this,_animator);
            var deathState = new DeathState(this, _animator);
            
            // Define Transitions
            At(freeLocomotionState, gridLocomotionState, new FuncPredicate(() => _playerInventory.HasWand)); // Placeholder condition, modifier pour que lorsqu'on récupère le baton on passe en mode grille
            At(gridLocomotionState, freeLocomotionState, new FuncPredicate(() => _playerInventory.HasWand)); 
            
            Any(deathState, new FuncPredicate(() => !_isAlive));
            
            _stateMachine.SetState(gridLocomotionState);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Start()
        {
            // Input Actions
            _moveAction = _playerInput.actions.FindAction("Move");
            _wandInteractionAction = _playerInput.actions.FindAction("WandInteraction");
            
            _wandInteractionAction.started += Interact;
            _wandInteractionAction.canceled -= Interact;
            
            _moveAction.started += GridMoveStep;
            _wandInteractionAction.canceled -= GridMoveStep;
        }

        public void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void HandleFreeMovement()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();
            PlayDirectionAnimation(moveInput);
            
            _rb.linearVelocity = moveInput * _speed;
        }
        
        private void Interact(InputAction.CallbackContext context)
        {
            if (!_playerInventory.HasWand) return; 
            
            RaycastHit2D hit = Physics2D.Raycast(_rb.position, transform.forward);

            if (hit.collider.TryGetComponent(out LivingTileEntity tileEntity))
            {
                return;
            }
            
            if(hit.collider.TryGetComponent(out StaticTileEntity staticTileEntity) && !_playerInventory.HasCube)
            {
                _playerInventory.ObtainCube();
                return;
            } 
            
            _playerInventory.RemoveCube();
            // Placer une tile
        }
        
        private void GridMoveStep(InputAction.CallbackContext context)
        {
            ResetVelocity();

            var moveDirection = context.ReadValue<Vector2>();
            
            Vector2 start = transform.position;
            Vector2 end = start + _tileStep * moveDirection;
            
            PlayDirectionAnimation(moveDirection);
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

