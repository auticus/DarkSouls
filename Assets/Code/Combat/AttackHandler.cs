using DarkSouls.Animation;
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
        private ICharacterController _characterController;

        private void Awake()
        {
            _animationHandler = GetComponent<AnimationHandler>();
            _inputHandler = GetComponent<InputHandler>();
            _inventory = GetComponent<CharacterInventory>();
            _characterController = GetComponent<ICharacterController>();

            _inputHandler.OnInputAttack += InputHandler_OnAttack;
            _inputHandler.OnInputHeavyAttack += InputHandler_OnHeavyAttack;
        }

        //Attack handles the right hand.  
        private void InputHandler_OnAttack()
        {
            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.IsAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedLightAttack, isInteractingAnimation: true);
        }

        //Heavy attack handles the right hand.
        private void InputHandler_OnHeavyAttack()
        {
            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.IsHeavyAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedHeavyAttack, isInteractingAnimation: true);
        }

        private void FinishAttack()
        {
            _characterController.IsAttacking = false;
            _characterController.IsHeavyAttacking = false;
            _animationHandler.FinishInteractionAnimation();
        }
    }
}
