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
        /// Gets or sets the Image of the control that this script is bound to.  Drag the image from the control over onto this via the editor.
        /// This will get set with the image of the item that populates the control.
        /// </summary>
        [field: SerializeField]
        public Image Icon { get; set; }

        /// <summary>
        /// Gets or sets the Inventory Item that this slot is associated with.
        /// </summary>
        [field: SerializeField]
        public Item InventoryItem { get; set; }

        /// <summary>
        /// Gets or sets the type of inventory this represents.
        /// </summary>
        [field: SerializeField]
        public InventoryType Type { get; set; }

        /// <summary>
        /// Populates the slot with the data and type required.
        /// </summary>
        /// <param name="item"></param>
        /// /// <param name="type"></param>
        public virtual void PopulateSlot(Item item, InventoryType type)
        {
            Icon.sprite = item.Icon;
            Icon.enabled = true;
            InventoryItem = item;
            Type = type;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Clears the slot of all data and disables the item.
        /// </summary>
        public virtual void ClearSlot()
        {
            Icon.sprite = null;
            Icon.enabled = false;
            InventoryItem = null;
            gameObject.SetActive(false);
        }
    }
}
