using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DarkSouls.Input
{
    public class InputHandler : MonoBehaviour, PlayerControls.IPlayerMovementActions, PlayerControls.IPlayerActionsActions
    {
        /// <summary>
        /// Gets a value indicating the movement on the X and Y axis of the gamepad or keyboard.
        /// </summary>
        public Vector2 MovementInput { get; private set; }
        public bool RollButtonPressed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public event Action OnInputRoll;

        private PlayerControls _inputActions;
        
        private void Start()
        {
            _inputActions = new PlayerControls();
            _inputActions.PlayerActions.SetCallbacks(this);
            _inputActions.PlayerMovement.SetCallbacks(this);

            _inputActions.PlayerActions.Enable();
            _inputActions.PlayerMovement.Enable();
        }

        private void OnDestroy()
        {
            _inputActions.PlayerActions.Disable();
            _inputActions.PlayerMovement.Disable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.action.ReadValue<Vector2>();
        }

        public void OnCamera(InputAction.CallbackContext context)
        {
            //Cinemachine is handling all of these, we just put this in here to make the interface happy
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RollButtonPressed = true;
                OnInputRoll?.Invoke();
            }

            if (context.canceled)
            {
                RollButtonPressed = false;
            }
        }
    }
}