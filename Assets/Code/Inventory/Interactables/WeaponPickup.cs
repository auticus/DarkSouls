using DarkSouls.Animation;
using DarkSouls.Locomotion.Player;
using UnityEngine;

namespace DarkSouls.Inventory
{
    /// <summary>
    /// Class that represents pick up items that represent <see cref="Weapon"/>.
    /// </summary>
    public class WeaponPickup : Interactable
    {
        /// <summary>
        /// Gets or sets the weapon that will be added to player inventory.
        /// </summary>
        [Tooltip("The weapon that will be added to player inventory")]
        [field: SerializeField]
        public Weapon PickupWeapon { get; set; }

        public override void Interact(PlayerController controller)
        {
            var inventory = controller.GetComponent<CharacterInventory>();
            var locomotion = controller.GetComponent<PlayerLocomotion>();
            var animationHandler = controller.GetComponentInChildren<AnimationHandler>();

            //the player will skid right on by when interacting if you don't stop them here
            locomotion.StopCharacterMovement();

            controller.OnInteractingAnimationCompleteDoThis = () => animationHandler.FinishInteractionAnimation();
            animationHandler.PlayTargetAnimation(AnimationHandler.PICKUP_ITEM, isInteractingAnimation: true);
            inventory.Weapons.Add(PickupWeapon);

            controller.PopupImageToPlayer(PickupWeapon.Icon, $"Picked up {PickupWeapon.Name}");
            Destroy(gameObject);
        }
    }
}
