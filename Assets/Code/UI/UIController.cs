using DarkSouls.Characters;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Controller placed on the Player that is responsible for updating the UI.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        private Healthbar _healthBar;
        private QuickSlots _quickSlots;
        private CharacterAttributes _characterAttributes;
        private WeaponSocketController _weaponSocketController;

        private void Awake()
        {
            _characterAttributes = GetComponent<CharacterAttributes>();
        }

        private void Start()
        {
            // health bar will reside on a canvas object outside of the player hierarchy.
            _healthBar = FindObjectOfType<Healthbar>(); //very inefficient method but there should only be one of these on the ui canvas
            _quickSlots = FindObjectOfType<QuickSlots>(); //same comment for healthbar.

            _characterAttributes.OnCharacterHealthChanged += OnCharacterHealthChanged;
            SetHealthBarMaximum(_characterAttributes.MaximumHealth);
            SetHealthBarValue(_characterAttributes.CurrentHealth);

            _weaponSocketController = GetComponent<WeaponSocketController>();
            _weaponSocketController.OnRightHandWeaponChanged += WeaponSocketController_OnRightHandWeaponChanged;
            _weaponSocketController.OnLeftHandWeaponChanged += WeaponSocketController_OnLeftHandWeaponChanged;
        }

        /// <summary>
        /// Will set the health bar maximum.  Will adjust health the current health if it is higher than the max.
        /// </summary>
        /// <param name="value"></param>
        private void SetHealthBarMaximum(int value)
        {
            _healthBar.Max = value;
            if (_healthBar.Current > _healthBar.Max) _healthBar.Current = _healthBar.Max;
        }

        /// <summary>
        /// Will attempt to set the health bar value.  If it is less than zero or greater than the max, it will clamp the values.
        /// </summary>
        /// <param name="value"></param>
        private void SetHealthBarValue(int value)
        {
            if (value < 0) value = 0;
            if (value > _healthBar.Max) value = _healthBar.Max;
            _healthBar.Current = value;
        }

        /// <summary>
        /// Event handler that fires off from the character attributes class whenever it registers a health change.
        /// </summary>
        /// <param name="currentHealth"></param>
        private void OnCharacterHealthChanged(int currentHealth)
        {
            SetHealthBarValue(currentHealth);
        }

        private void WeaponSocketController_OnRightHandWeaponChanged(Weapon weapon)
        {
            _quickSlots.UpdateRightHandSlotIcon(weapon);
        }

        private void WeaponSocketController_OnLeftHandWeaponChanged(Weapon weapon)
        {
            _quickSlots.UpdateLeftHandSlotIcon(weapon);
        }
    }
}