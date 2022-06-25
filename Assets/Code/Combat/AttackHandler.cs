using DarkSouls.Animation;
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
        private PlayerController _playerController;

        private void Awake()
        {
            _animationHandler = GetComponent<AnimationHandler>();
            _inputHandler = GetComponent<InputHandler>();
            _inventory = GetComponent<CharacterInventory>();
            _playerController = GetComponent<PlayerController>();

            _inputHandler.OnInputAttack += InputHandler_OnAttack;
            _inputHandler.OnInputHeavyAttack += InputHandler_OnHeavyAttack;
        }

        //Attack handles the right hand.  
        private void InputHandler_OnAttack()
        {
            _playerController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _playerController.IsAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedLightAttack, isInteractingAnimation: true);
        }

        //Heavy attack handles the right hand.
        private void InputHandler_OnHeavyAttack()
        {
            _playerController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _playerController.IsHeavyAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.OneHandedHeavyAttack, isInteractingAnimation: true);
        }

        private void FinishAttack()
        {
            _playerController.IsAttacking = false;
            _playerController.IsHeavyAttacking = false;
            _animationHandler.FinishInteractionAnimation();
        }
    }
}
