namespace DarkSouls.Characters
{
    public interface ICharacterController
    {
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
    }
}
