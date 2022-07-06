using DarkSouls.Characters;
using DarkSouls.Input;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Controller placed on the Player that is responsible for updating the UI.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        private StatusBar _healthBar;
        private StatusBar _staminaBar;
        private QuickSlots _quickSlots;

        private WindowsUI _windowsUIController;

        private CharacterAttributes _characterAttributes;
        private WeaponSocketController _weaponSocketController;
        private InputHandler _inputHandler;

        private void Awake()
        {
            _characterAttributes = GetComponent<CharacterAttributes>();
            _inputHandler = GetComponent<InputHandler>();
            _quickSlots = FindObjectOfType<QuickSlots>();
            _windowsUIController = FindObjectOfType<WindowsUI>();
            _weaponSocketController = GetComponent<WeaponSocketController>();

            var statusBars = FindObjectsOfType<StatusBar>();

            // status bars will reside on a canvas object outside of the player hierarchy.
            // there should only be one of each type in a scene
            foreach (var statusBar in statusBars)
            {
                if (statusBar.StatusBarType == StatusBarTypes.HealthBar)
                {
                    _healthBar = statusBar;
                    continue;
                }

                if (statusBar.StatusBarType == StatusBarTypes.StaminaBar)
                {
                    _staminaBar = statusBar;
                    continue;
                }

                Debug.LogWarning($"UIController receives a status bar of type {statusBar.StatusBarType} that has no known UI Element associated with it.");
            }
        }

        private void Start()
        {
            _characterAttributes.OnCharacterHealthChanged += OnCharacterHealthChanged;
            _characterAttributes.OnCharacterStaminaChanged += OnCharacterStaminaChanged;
            SetHealthBarMaximum(_characterAttributes.MaximumHealth);
            SetHealthBarValue(_characterAttributes.CurrentHealth);
            SetStaminaBarMaximum(_characterAttributes.MaximumStamina);
            SetStaminaBarValue(_characterAttributes.CurrentStamina);

            _weaponSocketController.OnRightHandWeaponChanged += WeaponSocketController_OnRightHandWeaponChanged;
            _weaponSocketController.OnLeftHandWeaponChanged += WeaponSocketController_OnLeftHandWeaponChanged;

            _inputHandler.OnInputStartMenu += InputHandler_StartMenuPressed;
        }

        #region Health
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
        #endregion

        #region Stamina

        private void SetStaminaBarMaximum(int value)
        {
            _staminaBar.Max = value;
            if (_staminaBar.Current > _staminaBar.Max) _staminaBar.Current = _staminaBar.Max;
        }

        private void SetStaminaBarValue(int value)
        {
            if (value < 0) value = 0;
            if (value > _staminaBar.Max) value = _staminaBar.Max;
            _staminaBar.Current = value;
        }

        private void OnCharacterStaminaChanged(int currentStamina)
        {
            SetStaminaBarValue(currentStamina);
        }
        #endregion

        #region Weapon Sockets
        private void WeaponSocketController_OnRightHandWeaponChanged(Weapon weapon)
        {
            _quickSlots.UpdateRightHandSlotIcon(weapon);
        }

        private void WeaponSocketController_OnLeftHandWeaponChanged(Weapon weapon)
        {
            _quickSlots.UpdateLeftHandSlotIcon(weapon);
        }
        #endregion

        #region UI Panels
        private void InputHandler_StartMenuPressed()
        {
            _windowsUIController.ToggleStartMenu();
        }

        #endregion
    }
}