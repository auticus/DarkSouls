using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class that is placed on Equipment Slot <see cref="GameObject"/> to manage the components within.
    /// </summary>
    public class EquipmentSlotContainer : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the button that represents Right Hand slot 1.  Drag in from the editor.
        /// </summary>
        [field: SerializeField] public EquipmentSlot RightHandButton1 { get; set; }

        /// <summary>
        /// Gets or sets the button that represents Right Hand slot 2.  Drag in from the editor.
        /// </summary>
        [field: SerializeField] public EquipmentSlot RightHandButton2 { get; set; }

        /// <summary>
        /// Gets or sets the button that represents Left Hand slot 1.  Drag in from the editor.
        /// </summary>
        [field: SerializeField] public EquipmentSlot LeftHandButton1 { get; set; }

        /// <summary>
        /// Gets or sets the button that represents Left Hand slot 2.  Drag in from the editor.
        /// </summary>
        [field: SerializeField] public EquipmentSlot LeftHandButton2 { get; set; }

        /// <summary>
        /// Will refresh all of the equipment slots based on the inventory passed in.
        /// </summary>
        /// <param name="inventory">The inventory that the slots will be based off of.</param>
        public void RefreshEquipmentSlots(CharacterInventory inventory)
        {
            RefreshRightHandSlots(inventory);
            RefreshLeftHandSlots(inventory);
        }

        private void RefreshRightHandSlots(CharacterInventory inventory)
        {
            for (var i = 0; i < inventory.RightHandWeaponSlots.Length; i++)
            {
                if (inventory.RightHandWeaponSlots[i] == null)
                {
                    if (i == 0) RightHandButton1.ClearSlot();
                    else RightHandButton2.ClearSlot();
                }
                else
                {
                    if (i == 0) RightHandButton1.PopulateSlot(inventory.RightHandWeaponSlots[i], InventoryType.Weapons);
                    else RightHandButton2.PopulateSlot(inventory.RightHandWeaponSlots[i], InventoryType.Weapons);
                }
            }
        }

        private void RefreshLeftHandSlots(CharacterInventory inventory)
        {
            for (var i = 0; i < inventory.LeftHandWeaponSlots.Length; i++)
            {
                if (inventory.LeftHandWeaponSlots[i] == null)
                {
                    if (i == 0) LeftHandButton1.ClearSlot();
                    else LeftHandButton2.ClearSlot();
                }
                else
                {
                    if (i == 0) LeftHandButton1.PopulateSlot(inventory.LeftHandWeaponSlots[i], InventoryType.Weapons);
                    else LeftHandButton2.PopulateSlot(inventory.LeftHandWeaponSlots[i], InventoryType.Weapons);
                }
            }
        }
    }
}
