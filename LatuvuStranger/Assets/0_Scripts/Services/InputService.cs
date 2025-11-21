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
        private InputAction _testAction;
        
        public InputAction Player => _moveAction;

        protected override void Awake()
        {
            base.Awake();

            _moveAction = _playerInput.actions.FindAction("Move");
            _interactionAction = _playerInput.actions.FindAction("Interact");
            _pauseAction = _playerInput.actions.FindAction("Pause");
            _testAction = _playerInput.actions.FindAction("Test");

            _playerInput.actions.Enable();
        }

        public void RegisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            _moveAction.started += callback;
        }

        public void UnregisterMoveAction(Action<InputAction.CallbackContext> callback)
        {
            _moveAction.started -= callback;
        }

        public void RegisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _interactionAction.started += callback;
        }

        public void UnregisterWandInteraction(Action<InputAction.CallbackContext> callback)
        {
            _interactionAction.started -= callback;
        }

        public void RegisterPauseAction(Action<InputAction.CallbackContext> callback)
        {
            _pauseAction.started += callback;
        }

        public void UnregisterPauseAction(Action<InputAction.CallbackContext> callback)
        {
            _pauseAction.started -= callback;
        }
        
        public void RegisterTestAction(Action<InputAction.CallbackContext> callback)
        {
            _testAction.started += callback;
        }

        public void UnregisterTestAction(Action<InputAction.CallbackContext> callback)
        {
            _testAction.started -= callback;
        }
        
        public void EnableInput()
        {
            _playerInput.SwitchCurrentActionMap("Player");
        }

        public void DisableInput()
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }
    }
}