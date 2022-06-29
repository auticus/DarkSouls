using System;

namespace DarkSouls.Characters
{
    public interface ICharacterController
    {
        /// <summary>
        /// Gets or sets a value indicating how long the player has been in the air.
        /// </summary>
        float AerialTimer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is in the air.
        /// </summary>
        bool IsAerial { get; set; } //todo: why not just make this IsGrounded = false?

        /// <summary>
        /// Gets or sets a value indicating that the player is performing an attack with their right hand.
        /// </summary>
        bool IsAttacking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is on the ground.
        /// </summary>
        bool IsGrounded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is performing a heavy attack with their right hand.
        /// </summary>
        bool IsHeavyAttacking { get; set; }

        /// <summary>
        /// Gets a value indicating if the animator is in an animation state that cannot be changed until completion.
        /// </summary>
        bool IsInteracting { get; set; }

        /// <summary>
        /// Gets or sets an action that will execute when an interacting animation completes.
        /// </summary>
        Action OnInteractingAnimationCompleteDoThis { get; set; }

        /// <summary>
        /// Damages the character for the value passed.
        /// </summary>
        /// <param name="damage"></param>
        void DamageCharacter(int damage);

        /// <summary>
        /// Called by the animator to notify the controller that the animation is in a state where the weapon's damage collider should be enabled.
        /// </summary>
        void EnableAttackingWeaponCollider();

        /// <summary>
        /// Called by the animator to notify the controller that the animation is in a state where the weapon's damage collider should be disabled.
        /// </summary>
        void DisableAttackingWeaponCollider();

        /// <summary>
        /// Called by the animator to finish an interactive animation.
        /// </summary>
        void FinishInteractiveAnimation();
    }
}
