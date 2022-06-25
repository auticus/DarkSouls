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
        public bool AttackButtonPressed { get; private set; }
        public bool HeavyAttackButtonPressed { get; private set; }

        /// <summary>
        /// Fires when the Roll button is invoked.
        /// </summary>
        public event Action OnInputRoll;

        /// <summary>
        /// Fires when the Attack button is pressed.
        /// </summary>
        public event Action OnInputAttack;

        /// <summary>
        /// Fires when the Heavy Attack button is pressed.
        /// </summary>
        public event Action OnInputHeavyAttack;

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

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackButtonPressed = true;
                OnInputAttack?.Invoke();
            }

            if (context.canceled)
            {
                AttackButtonPressed = false;
            }
        }

        public void OnHeavyAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                HeavyAttackButtonPressed = true;
                OnInputHeavyAttack?.Invoke();
            }

            if (context.canceled)
            {
                HeavyAttackButtonPressed = false;
            }
        }
    }
}