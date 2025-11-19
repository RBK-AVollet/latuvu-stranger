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
        [SerializeField] private float _gridMoveTime = 0.2f;
        
        [SerializeField] private Animator _animator;
        
        private InputAction _moveAction;
        
        private StateMachine _stateMachine;
        
        private bool _isMovingGrid = false; 
        private bool _useGridMovement = true; // TEMP
        
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
            
            // State Machine
            _stateMachine = new StateMachine();
            
            // Declare States
            var freeLocomotionState = new FreeLocomotionState(this, _animator);
            var gridLocomotionState = new GridLocomotionState(this,_animator);
            var deathState = new DeathState(this, _animator);
            
            // Define Transitions
            At(freeLocomotionState, gridLocomotionState, new FuncPredicate(() => _useGridMovement)); // Placeholder condition, modifier pour que lorsqu'on récupère le baton on passe en mode grille
            At(gridLocomotionState, freeLocomotionState, new FuncPredicate(() => !_useGridMovement)); 
            
            Any(deathState, new FuncPredicate(() => !_isAlive));
            
            _stateMachine.SetState(gridLocomotionState);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Start()
        {
            // Input Actions
            _moveAction = _playerInput.actions.FindAction("Move");
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
        
        public void HandleGridMovement()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();
            
            PlayDirectionAnimation(moveInput);
            
            if (_isMovingGrid) return;
            
            if (moveInput.sqrMagnitude < 0.5f) return;

            Vector2 direction = Vector2.zero;
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                direction = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                direction = new Vector2(0, Mathf.Sign(moveInput.y));

            StartCoroutine(GridMoveStep(direction));
        }
        
        private IEnumerator GridMoveStep(Vector2 direction)
        {
            _isMovingGrid = true;
            ResetVelocity();

            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)direction;

            float elapsed = 0f;

            while (elapsed < _gridMoveTime)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / _gridMoveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            _isMovingGrid = false;
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

