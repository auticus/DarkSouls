using System;

namespace DarkSouls.Characters
{
    public interface ICharacterController
    {
        /// <summary>
        /// Model that holds all of the character's data state.
        /// </summary>
        CharacterState State { get; }

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
