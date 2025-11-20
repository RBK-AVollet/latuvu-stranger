using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace Latuvu
{
    public class InputService : Singleton<InputService>
    {
        [SerializeField] private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _interactionAction;

        protected override void Awake()
        {
            base.Awake();
            
            _moveAction = _playerInput.actions.FindAction("Move");
            _interactionAction = _playerInput.actions.FindAction("Interact");
        }

        public void RegisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            Debug.Log("RegisterMoveAction");
            
            _moveAction.started += callback;
        }

        public void UnregisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            _moveAction.canceled -= callback;
        }
        
        public void RegisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _interactionAction.started += callback;
        }
        
        public void UnregisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _interactionAction.canceled -= callback;
        }
    }
}
