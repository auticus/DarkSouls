using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class that sits over the WindowsUI game object and works with the panel objects within.
    /// </summary>
    public class WindowsUI : MonoBehaviour
    {
        private bool _startMenuVisible;
        private bool _inventoryVisible;

        private CharacterInventory _characterInventory;
        private InventorySlotContainer _weaponsInventoryContainer;
        private EquipmentSlotContainer _equipmentContainer;
        private CharacterDoll _characterDoll;

        [SerializeField] private GameObject Hud;
        [SerializeField] private GameObject StartMenu;
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject EquipmentPanel;
        [SerializeField] private GameObject WeaponsInventoryContainer;
        [SerializeField] private GameObject CharacterDollContainer;
        
        private GameObject _playerCharacter; //used for paper dolling

        private void Awake()
        {
            var characters = GameObject.FindGameObjectsWithTag("Player"); //there should only ever be one of these and this has all the scripts loaded on it
            if (characters == null || characters.Length > 1)
            {
                Debug.LogWarning("The player character was either not found or there is more than one game object in the scene with the tag Player.");
            }
            else
            {
                _playerCharacter = characters[0];
                _characterInventory = _playerCharacter.GetComponent<CharacterInventory>();
            }

            _weaponsInventoryContainer = WeaponsInventoryContainer.GetComponent<InventorySlotContainer>();
            _equipmentContainer = EquipmentPanel.GetComponent<EquipmentSlotContainer>();
            _characterDoll = CharacterDollContainer.GetComponent<CharacterDoll>();
        }

        private void Start()
        {
            if (Hud == null || StartMenu == null || InventoryPanel == null || EquipmentPanel == null ||
                WeaponsInventoryContainer == null || CharacterDollContainer == null)
            {
                Debug.LogWarning("WindowsUI was not wired up fully - there are components that are null.");
            }
        }

        /// <summary>
        /// Toggles the start menu off and on.
        /// </summary>
        public void ToggleStartMenu()
        {
            // if the inventory is visible, we aren't showing the start menu - we need to just close inventory out
            if (_inventoryVisible)
            {
                ToggleInventoryPanel();
                return;
            }

            _startMenuVisible = !_startMenuVisible;
            StartMenu.SetActive(_startMenuVisible);
        }

        /// <summary>
        /// Called by editor button OnClick to toggle Inventory.
        /// </summary>
        public void ToggleInventoryPanel()
        {
            if (_startMenuVisible) ToggleStartMenu();

            _inventoryVisible = !_inventoryVisible;
            
            InventoryPanel.SetActive(_inventoryVisible);
            EquipmentPanel.SetActive(_inventoryVisible);
            
            if (_inventoryVisible)
            {
                RefreshInventory();
                _characterDoll.AddCharacterAndScrubScripts(_playerCharacter);
                Hud.SetActive(false);
            }
            else
            {
                _characterDoll.RemoveCharacter();
                Hud.SetActive(true);
            }

        }
        
        /// <summary>
        /// Refreshes all inventory panels with the current inventory passed.
        /// </summary>
        public void RefreshInventory()
        {
            _weaponsInventoryContainer.RefreshContainer(_characterInventory.Weapons);
            _equipmentContainer.RefreshEquipmentSlots(_characterInventory);
        }
    }
}
