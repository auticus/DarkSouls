using System;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Class pertaining to the attribute system of the characters.
    /// </summary>
    public class CharacterAttributes : MonoBehaviour
    {
        private const int DEFAULT_ATTRIBUTE_VALUE = 10;
        private const int HEALTH_PIP_VALUE = 5;

        /// <summary>
        /// Event that fires whenever the health of the player changed and sends the current health value.
        /// </summary>
        public event Action<int> OnCharacterHealthChanged;

        /// <summary>
        /// Gets or sets a value that is how healthy the character is.
        /// </summary>
        [field: SerializeField]
        public int Constitution { get; set; } = DEFAULT_ATTRIBUTE_VALUE;

        /// <summary>
        /// Gets a value indicating how many hit points the character can have.
        /// </summary>
        [field: SerializeField] [Tooltip("The maximum hit points the character has")]
        public int MaximumHealth { get; private set; }

        /// <summary>
        /// Gets a value indicating what the current hit points of the character is.
        /// </summary>
        /// a better way would be to fire off the health changed in the setter but if you do that Unity
        /// will not let you serialize the field so it will not show up in the designer.
        [field: SerializeField]
        [Tooltip("The current hit points the character has")]
        public int CurrentHealth { get; set; } 

        private void Awake()
        {
            InitializeHitPoints();
        }

        /// <summary>
        /// Damages the character for a value passed.
        /// </summary>
        /// <param name="damage">The amount of damage applied to the character.</param>
        public void DamageCharacter(int damage)
        {
            if (CurrentHealth == 0) return;

            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
            OnCharacterHealthChanged?.Invoke(CurrentHealth);
        }

        private void InitializeHitPoints()
        {
            MaximumHealth = Constitution * HEALTH_PIP_VALUE;
            CurrentHealth = MaximumHealth;
        }
    }
}
