using DarkSouls.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSouls.UI
{
    public class QuickSlots : MonoBehaviour
    {
        // These four images must be set on the editor with the icon Images from the UI Canvas.  They should not be changed anywhere else
        // in code.  Unfortunately private setting these will make it so that you cannot create them frm the editor and drag them over.  
        // we would need to create a more complicated system to snake out and find these items using tags or layers or some other method of identifying them.
        public Image LeftHandSlotIcon;
        public Image RightHandSlotIcon;
        public Image QuickSlotIcon;
        public Image SpellSlotIcon;

        /// <summary>
        /// Updates the Left Hand Slot with the weapon's icon that belongs to the weapon passed.
        /// </summary>
        /// <param name="weapon"></param>
        public void UpdateLeftHandSlotIcon(Weapon weapon)
            => UpdateIcon(ref LeftHandSlotIcon, weapon?.Icon);

        /// <summary>
        /// Updates the Right Hand Slot with the weapon's icon that belongs to the weapon passed.
        /// </summary>
        /// <param name="weapon"></param>
        public void UpdateRightHandSlotIcon(Weapon weapon)
            => UpdateIcon(ref RightHandSlotIcon, weapon?.Icon);
        

        private void UpdateIcon(ref Image uiIcon, Sprite targetIcon)
        {
            if (targetIcon != null)
            {
                uiIcon.sprite = targetIcon;
                uiIcon.enabled = true;
            }
            else
            {
                uiIcon.sprite = null;
                uiIcon.enabled = false;
            }
        }
    }
}
