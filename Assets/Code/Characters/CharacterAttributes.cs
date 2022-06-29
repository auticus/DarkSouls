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
        [field: SerializeField]
        [Tooltip("The current hit points the character has")]
        public int CurrentHealth { get; private set; }

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
        }

        private void InitializeHitPoints()
        {
            MaximumHealth = Constitution * HEALTH_PIP_VALUE;
            CurrentHealth = MaximumHealth;
        }
    }
}
