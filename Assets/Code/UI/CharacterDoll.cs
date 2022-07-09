using System.Linq;
using DarkSouls.Animation;
using DarkSouls.Combat;
using DarkSouls.Input;
using DarkSouls.Inventory;
using DarkSouls.Locomotion.Player;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class used to manage the Character Doll on the character screen
    /// </summary>
    public class CharacterDoll : MonoBehaviour
    {
        private int _characterDollLayer;

        private void Awake()
        {
            _characterDollLayer = LayerMask.NameToLayer("Character Doll");
        }
        /// <summary>
        /// Adds the character and deactivates all scripts on it.
        /// </summary>
        public void AddCharacterAndScrubScripts(GameObject character)
        {
            var doll = Instantiate(character, parent: this.transform);
            doll.tag = "Doll";
            doll.SetLayer(_characterDollLayer, includeChildren: true, includeInactive: false);

            var rigidBody = doll.GetComponent<Rigidbody>();
            var capsule = doll.GetComponent<CapsuleCollider>();
            var animationHandler = doll.GetComponent<AnimationHandler>();
            var inputHandler = doll.GetComponent<InputHandler>();
            var playerController = doll.GetComponent<PlayerController>();
            var playerLocomotion = doll.GetComponent<PlayerLocomotion>();
            var target = doll.GetComponent<Target>();
            var uiController = doll.GetComponent<UIController>();
            var animator = doll.GetComponent<Animator>();
            var inventory = doll.GetComponent<CharacterInventory>();

            capsule.enabled = false;
            rigidBody.useGravity = false;
            animationHandler.enabled = false;
            inputHandler.enabled = false;
            playerController.enabled = false;
            playerLocomotion.enabled = false;
            target.enabled = false;
            uiController.enabled = false;

            SetDollAnimationStates(animator, inventory);
        }

        public void RemoveCharacter()
        {
            var dollTransform = transform.Cast<Transform>().FirstOrDefault(
                child => child.gameObject.CompareTag("Doll"));

            if (dollTransform == null) return;
            Destroy(dollTransform.gameObject);
        }

        private void SetDollAnimationStates(Animator animator, CharacterInventory inventory)
        {
            animator.CrossFade(inventory.LeftHand != null ? inventory.LeftHand.LeftHandIdle : AnimationHandler.LEFT_ARM_IDLE_EMPTY, 0.2f);
            animator.CrossFade(inventory.RightHand != null ? inventory.RightHand.RightHandIdle : AnimationHandler.RIGHT_ARM_IDLE_EMPTY, 0.2f);
        }
    }
}
