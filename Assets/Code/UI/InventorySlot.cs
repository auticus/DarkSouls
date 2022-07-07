using DarkSouls.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class that sits on an inventory slot <see cref="GameObject"/>
    /// </summary>
    public class InventorySlot : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the Icon of the control that this script is bound to.  Drag over from the editor.
        /// </summary>
        [field: SerializeField]
        public Image Icon { get; set; }

        /// <summary>
        /// Gets or sets the Inventory Item that this slot is associated with.
        /// </summary>
        [field: SerializeField]
        public Item InventoryItem { get; set; }

        /// <summary>
        /// Populates the slot with the data and type required.
        /// </summary>
        /// <param name="item"></param>
        public void PopulateSlot(Item item)
        {
            Icon.sprite = item.Icon;
            Icon.enabled = true;
            InventoryItem = item;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Clears the slot of all data and disables the item.
        /// </summary>
        public void ClearSlot()
        {
            Icon.sprite = null;
            Icon.enabled = false;
            InventoryItem = null;
            gameObject.SetActive(false);
        }
    }
}
