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
        private InputAction _pauseAction;

        protected override void Awake()
        {
            base.Awake();
            
            _moveAction = _playerInput.actions.FindAction("Move");
            _interactionAction = _playerInput.actions.FindAction("Interact");
            _pauseAction = _playerInput.actions.FindAction("Pause");
            
            _playerInput.actions.Enable();
        }

        public void RegisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            Debug.Log($"RegisterMoveAction {callback.Method.Name}");
            
            _moveAction.started += callback;
        }

        public void UnregisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            _moveAction.started -= callback;
        }
        
        public void RegisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            Debug.Log($"RegisterWandInteraction {callback.Method.Name}");
            
            _interactionAction.started += callback;
        }
        
        public void UnregisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _interactionAction.started -= callback;
        }
        
        public void RegisterPauseAction(Action<InputAction.CallbackContext> callback)
        {
            Debug.Log($"RegisterWandInteraction {callback.Method.Name}");
            
            _pauseAction.started += callback;
        }
        
        public void UnregisterPauseAction(Action<InputAction.CallbackContext> callback)
        {
            _pauseAction.started -= callback;
        }
        
        public void EnableInput()
        {
            _playerInput.actions.Enable();
        }

        public void DisableInput()
        {
            _playerInput.actions.Disable();
        }
    }
}
