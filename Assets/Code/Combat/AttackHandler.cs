﻿using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Input;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.Combat
{
    public class AttackHandler : MonoBehaviour
    {
        private AnimationHandler _animationHandler;
        private InputHandler _inputHandler;
        private CharacterInventory _inventory;
        private PlayerController _characterController;

        private void Awake()
        {
            _animationHandler = GetComponent<AnimationHandler>();
            _inputHandler = GetComponent<InputHandler>();
            _inventory = GetComponent<CharacterInventory>();
            _characterController = GetComponent<PlayerController>();

            _inputHandler.OnInputAttack += InputHandler_OnAttack;
            _inputHandler.OnInputHeavyAttack += InputHandler_OnHeavyAttack;
        }

        //Attack handles the right hand.  
        private void InputHandler_OnAttack()
        {
            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.State.IsAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedLightAttack, isInteractingAnimation: true);
        }

        //Heavy attack handles the right hand.
        private void InputHandler_OnHeavyAttack()
        {
            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.State.IsHeavyAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedHeavyAttack, isInteractingAnimation: true);
        }

        private void FinishAttack()
        {
            _characterController.State.IsAttacking = false;
            _characterController.State.IsHeavyAttacking = false;
            _animationHandler.FinishInteractionAnimation();
        }
    }
}