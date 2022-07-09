using System.Collections.Generic;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class that is placed on Inventory Slot Container <see cref="GameObject"/> to manage the slots contained within.
    /// </summary>
    public class InventorySlotContainer : MonoBehaviour
    {
        private readonly List<InventorySlot> _containerSlots = new List<InventorySlot>();

        /// <summary>
        /// Gets or sets the prefab that will be used to instantiate this collection.  Wire this up from the designer.
        /// </summary>
        [field: SerializeField]
        public GameObject InventorySlotPrefab { get; set; }
        
        /// <summary>
        /// Refreshes the container with the inventory passed within.
        /// </summary>
        /// <param name="inventory"></param>
        public void RefreshContainer(IEnumerable<Item> inventory)
        {
            // at any given moment, this collection can grow, and then the player can get rid of items which will cause squares to need deleted.
            // deleted in this instance is simply disabling them as when you clear an inventory slot it turns itself off.
            int index = 0;
            if (InventorySlotPrefab == null)
            {
                Debug.LogWarning("Inventory Slot Container :: RefreshContainer - InventorySlotPrefab has not been assigned");
                return;
            }

            foreach (var item in inventory)
            {
                var type = item.ConvertItemToType();
                if (index < _containerSlots.Count)
                {
                    _containerSlots[index].PopulateSlot(item, type);
                }
                else
                {
                    var newSlotGameObject = Instantiate(InventorySlotPrefab, gameObject.transform);
                    var newSlot = newSlotGameObject.GetComponent<InventorySlot>();
                    newSlot.PopulateSlot(item, type);
                    _containerSlots.Add(newSlot);
                }

                index++;
            }


            //now take where we left off, if the container slots are greater that means the remainder of those need to be cleared
            for (var i = index; i < _containerSlots.Count; index++)
            {
                _containerSlots[i].ClearSlot();
            }
        }
    }
}
