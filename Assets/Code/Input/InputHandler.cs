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
        public bool JumpButtonPressed { get; private set; }
        public bool InteractButtonPressed { get; private set; }
        public bool AttackButtonPressed { get; private set; }
        public bool HeavyAttackButtonPressed { get; private set; }
        public bool RightHandCyclePressed { get; private set; }
        public bool LeftHandCyclePressed { get; private set; }

        /// <summary>
        /// Fires when the Roll button is invoked.
        /// </summary>
        public event Action OnInputRoll;

        /// <summary>
        /// Fires when the Jump button is invoked.
        /// </summary>
        public event Action OnInputJump;

        /// <summary>
        /// Fires when the Interaction button is invoked.
        /// </summary>
        public event Action OnInputInteraction;

        /// <summary>
        /// Fires when the Attack button is pressed.
        /// </summary>
        public event Action OnInputAttack;

        /// <summary>
        /// Fires when the Heavy Attack button is pressed.
        /// </summary>
        public event Action OnInputHeavyAttack;

        /// <summary>
        /// Fires when the right hand cycle button is pressed.
        /// </summary>
        public event Action OnInputCycleRightHand;

        /// <summary>
        /// Fires when the left hand cycle button is pressed.
        /// </summary>
        public event Action OnInputCycleLeftHand;

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

        #region Interface Events from Input Controller
        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.action.ReadValue<Vector2>();
        }

        public void OnCamera(InputAction.CallbackContext context)
        {
            //Cinemachine is handling all of these, we just put this in here to make the interface happy
        }

        public void OnJump(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => JumpButtonPressed = value, ref OnInputJump);

        public void OnRoll(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => RollButtonPressed = value, ref OnInputRoll);
        
        public void OnAttack(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => AttackButtonPressed = value, ref OnInputAttack);
        
        public void OnHeavyAttack(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => HeavyAttackButtonPressed = value, ref OnInputHeavyAttack);

        public void OnCycleRightHand(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => RightHandCyclePressed = value, ref OnInputCycleRightHand);

        public void OnCycleLeftHand(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => LeftHandCyclePressed = value, ref OnInputCycleLeftHand);

        public void OnInteract(InputAction.CallbackContext context)
            => ProcessInputEvent(context, value => InteractButtonPressed = value, ref OnInputInteraction);

        private void ProcessInputEvent(InputAction.CallbackContext context, Action<bool> boolFlagProccessingDirective, ref Action eventAction)
        {
            if (context.performed)
            {
                boolFlagProccessingDirective(true);
                eventAction?.Invoke();
            }

            if (context.canceled)
            {
                boolFlagProccessingDirective(false);
            }
        }
        #endregion
    }
}