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

        [SerializeField] private GameObject StartMenu;
        [SerializeField] private GameObject InventoryPanel;

        /// <summary>
        /// Toggles the start menu off and on.
        /// </summary>
        public void ToggleStartMenu()
        {
            // will not toggle the start menu if the other panels are active
            if (_inventoryVisible) return;

            _startMenuVisible = !_startMenuVisible;
            StartMenu.SetActive(_startMenuVisible);
        }

        public void ToggleInventoryPanel()
        {
            if (_startMenuVisible) ToggleStartMenu();

            _inventoryVisible = !_inventoryVisible;
            InventoryPanel.SetActive(_inventoryVisible);
        }

        public void ToggleEquipmentPanel()
        {
            //todo
        }
    }
}
