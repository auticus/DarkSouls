using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Input;
using DarkSouls.Inventory;
using UnityEditorInternal;
using UnityEngine;

namespace DarkSouls.Combat
{
    public class AttackHandler : MonoBehaviour
    {
        private AnimationHandler _animationHandler;
        private InputHandler _inputHandler;
        private CharacterInventory _inventory;
        private PlayerController _characterController;
        private int _currentAttackAnimationIndex;

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
            if (_characterController.State.IsInteracting)
            {
                // if i'm interacting then return out if I'm either not attacking (doing something else like rolling) or I AM attacking but not able to combo
                if ((!_characterController.State.IsAttacking) ||
                    (_characterController.State.IsAttacking && !_characterController.State.IsAbleToCombo)) return;
            }

            if (_characterController.State.IsAttacking)
            {
                // we are allowed to combo so boost the index up if possible
                if (_currentAttackAnimationIndex + 1 == _inventory.RightHand.LightAttackAnimations.Count) return; // we're at the end of the chain already
                _currentAttackAnimationIndex++;
            }

            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.State.IsAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.LightAttackAnimations[_currentAttackAnimationIndex].AnimationName, isInteractingAnimation: true);
        }

        //Heavy attack handles the right hand.
        private void InputHandler_OnHeavyAttack()
        {
            if (_characterController.State.IsInteracting)
            {
                // if i'm interacting then return out if I'm either not attacking (doing something else like rolling) or I AM attacking but not able to combo
                if ((!_characterController.State.IsHeavyAttacking) ||
                    (_characterController.State.IsHeavyAttacking && !_characterController.State.IsAbleToCombo)) return;
            }

            if (_characterController.State.IsAttacking)
            {
                // we are allowed to combo so boost the index up if possible
                if (_currentAttackAnimationIndex + 1 == _inventory.RightHand.HeavyAttackAnimations.Count) return; // we're at the end of the chain already
                _currentAttackAnimationIndex++;
            }

            _characterController.OnInteractingAnimationCompleteDoThis = FinishAttack;
            _characterController.State.IsHeavyAttacking = true;
            _animationHandler.PlayTargetAnimation(_inventory.RightHand.HeavyAttackAnimations[_currentAttackAnimationIndex].AnimationName, isInteractingAnimation: true);
        }

        private void FinishAttack()
        {
            _characterController.State.IsAttacking = false;
            _characterController.State.IsHeavyAttacking = false;
            _currentAttackAnimationIndex = 0;
            _animationHandler.FinishInteractionAnimation();
        }
    }
}
