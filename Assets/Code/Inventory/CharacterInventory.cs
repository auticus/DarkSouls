using UnityEngine;

namespace DarkSouls.Inventory
{
    public class CharacterInventory : MonoBehaviour
    {
        private WeaponSocketController _weaponSocketController;

        /// <summary>
        /// What weapon is loaded in the right hand.
        /// </summary>
        public Weapon RightHand;

        /// <summary>
        /// What weapon is loaded in the left hand.
        /// </summary>
        public Weapon LeftHand;

        private void Awake()
        {
            _weaponSocketController = GetComponentInChildren<WeaponSocketController>();
        }

        private void Start()
        {
            _weaponSocketController.LoadRightHandSocketWithWeapon(RightHand);
            _weaponSocketController.LoadLeftHandSocketWithWeapon(LeftHand);
        }
    }
}
