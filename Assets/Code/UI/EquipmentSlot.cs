using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// A specialized type of <see cref="InventorySlot"/> that is for Equipment slots that is placed on the slot button.
    /// </summary>
    public class EquipmentSlot : InventorySlot
    {
        [field: SerializeField] public EquipmentType SlotType { get; set; }

        /// <summary>
        /// Populates the slot with the data and type required.
        /// </summary>
        /// <param name="item"></param>
        /// /// <param name="type"></param>
        public override void PopulateSlot(Item item, InventoryType type)
        {
            Icon.sprite = item.Icon;
            Icon.enabled = true;
            InventoryItem = item;
            Type = type;
        }

        /// <summary>
        /// Clears the slot of all data and disables the item.
        /// </summary>
        public override void ClearSlot()
        {
            Icon.sprite = null;
            Icon.enabled = false;
            InventoryItem = null;
        }
    }
}
