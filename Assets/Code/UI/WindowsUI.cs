using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private InventorySlotContainer _weaponsContainer;

        [SerializeField] private GameObject Hud;
        [SerializeField] private GameObject StartMenu;
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject WeaponsInventoryContainer;

        private void Awake()
        {
            _weaponsContainer = WeaponsInventoryContainer.GetComponent<InventorySlotContainer>();
            _characterInventory = FindObjectOfType<CharacterInventory>();
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
            
            if (_inventoryVisible)
            {
                RefreshInventory();
                Hud.SetActive(false);
            }
            else
            {
                Hud.SetActive(true);
            }
        }

        /// <summary>
        /// Called by editor OnClick to toggle Equipment.
        /// </summary>
        public void ToggleEquipmentPanel()
        {
            //todo
        }

        /// <summary>
        /// Refreshes all inventory panels with the current inventory passed.
        /// </summary>
        /// <param name="weapons">A list of all of the weapons in the player inventory.</param>
        public void RefreshInventory()
        {
            _weaponsContainer.RefreshContainer(_characterInventory.Weapons);
        }
    }
}
