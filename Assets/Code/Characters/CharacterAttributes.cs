using System;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Class pertaining to the attribute system of the characters.
    /// </summary>
    public class CharacterAttributes : MonoBehaviour
    {
        private const int DEFAULT_ATTRIBUTE_VALUE = 8;
        private const int PIP_VALUE = 5;

        /// <summary>
        /// Event that fires whenever the health of the player changed and sends the current health value.
        /// </summary>
        public event Action<int> OnCharacterHealthChanged;

        /// <summary>
        /// Event that fires whenever the stamina of the player changed and sends the current stamina value.
        /// </summary>
        public event Action<int> OnCharacterStaminaChanged;

        /// <summary>
        /// Gets or sets a value that determines how agile a character is, which determines their speed and ranged attacks.
        /// </summary>
        [field: SerializeField]
        public int Agility { get; set; } = DEFAULT_ATTRIBUTE_VALUE;

        /// <summary>
        /// Gets or sets a value that is how healthy the character is.
        /// </summary>
        [field: SerializeField]
        public int Constitution { get; set; } = DEFAULT_ATTRIBUTE_VALUE;

        /// <summary>
        /// Gets or sets a value that determines how much stamina a character has before becoming exhausted.
        /// </summary>
        [field: SerializeField]
        public int Endurance { get; set; } = DEFAULT_ATTRIBUTE_VALUE;

        /// <summary>
        /// Gets or sets a value that determines how strong a character is, which determines their force, carry capacity, jumping etc.
        /// </summary>
        [field: SerializeField]
        public int Strength { get; set; } = DEFAULT_ATTRIBUTE_VALUE;

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

        /// <summary>
        /// Gets a value indicating how many points can exist in the player stamina pool.
        /// </summary>
        [field: SerializeField]
        public int MaximumStamina { get; private set; }

        /// <summary>
        /// Gets a value indicating what the current stamina of the character is.
        /// </summary>
        [field: SerializeField]
        public int CurrentStamina { get; set; }

        private void Awake()
        {
            InitializeHitPoints();
            InitializeStaminaPool();
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

        public void DrainStamina(int staminaDrain)
        {
            if (CurrentStamina == 0) return; //already exhausted

            CurrentStamina -= staminaDrain;
            if (CurrentStamina < 0) CurrentStamina = 0;
            OnCharacterStaminaChanged?.Invoke(CurrentStamina);

            // todo: invoke an on exhausted event to trigger an exhausted state
        }

        private void InitializeHitPoints()
        {
            MaximumHealth = Constitution * PIP_VALUE;
            CurrentHealth = MaximumHealth;
        }

        private void InitializeStaminaPool()
        {
            MaximumStamina = Endurance * PIP_VALUE;
            CurrentStamina = MaximumStamina;
        }
    }
}
