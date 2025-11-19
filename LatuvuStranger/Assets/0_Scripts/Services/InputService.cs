using UnityEngine.InputSystem;
using System;
using UnityEngine;

namespace Latuvu
{
    public class InputService : Singleton<InputService>
    {
        [SerializeField] private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _wandInteractionAction;

        protected override void Awake()
        {
            _moveAction = _playerInput.actions.FindAction("Move");
            _wandInteractionAction = _playerInput.actions.FindAction("WandInteraction");
            
            Debug.Log(_moveAction.enabled);
            Debug.Log(_wandInteractionAction.enabled);
            
            Debug.Log("Initializing InputService");
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
            _wandInteractionAction.started += callback;
        }
        
        public void UnregisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _wandInteractionAction.canceled -= callback;
        }
    }
}
