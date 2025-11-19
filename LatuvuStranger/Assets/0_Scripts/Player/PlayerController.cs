using UnityEngine.InputSystem;
using UnityEngine;

namespace Latuvu
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        
        [SerializeField] private float _speed = 5f;
        
        private InputAction _moveAction;
        private Rigidbody2D _rb;
        
        private StateMachine _stateMachine;

        private void Awake()
        {
            // State Machine
            _stateMachine = new StateMachine();
            
            // Declare States
            var freeLocomotionState = new FreeLocomotionState(this);
            var gridLocomotionState = new GridLocomotionState(this);
            
            // Define Transitions
            At(freeLocomotionState, gridLocomotionState, new FuncPredicate(() => true)); // Placeholder condition, modifier pour que lorsqu'on récupère le baton on passe en mode grille
            
            _stateMachine.SetState(freeLocomotionState);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            
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
            
            _rb.linearVelocity = moveInput * _speed;
            // Noop
        }
        
        public void HandleGridMovement()
        {
            // Noop
        }
    }
}
