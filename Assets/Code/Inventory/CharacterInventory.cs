using DarkSouls.Characters;
using DarkSouls.Input;
using UnityEngine;

namespace DarkSouls.Inventory
{
    /// <summary>
    /// Inventory class used to hold information pertaining to all of the equipment that a character holds.
    /// </summary>
    public class CharacterInventory : MonoBehaviour
    {
        private const int MAX_WEAPON_SLOTS_PER_HAND = 2;
        private int _activeRightHandSlotIndex = 0;
        private int _activeLeftHandSlotIndex = 0;

        private InputHandler _inputHandler;
        private WeaponSocketController _weaponSocketController;

        /// <summary>
        /// What weapon is loaded in the right hand.
        /// </summary>
        public Weapon RightHand;

        /// <summary>
        /// What weapon is loaded in the left hand.
        /// </summary>
        public Weapon LeftHand;

        /// <summary>
        /// Gets available weapon slots and holds what weapons are actively able to be switched in the right hand.
        /// </summary>
        public Weapon[] RightHandWeaponSlots = new Weapon[MAX_WEAPON_SLOTS_PER_HAND];

        /// <summary>
        /// Gets available weapon slots and holds what weapons are actively able to be switched in the left hand.
        /// </summary>
        public Weapon[] LeftHandWeaponSlots = new Weapon[MAX_WEAPON_SLOTS_PER_HAND];

        /// <summary>
        /// The unarmed weapon that will be used if a hand does not contain a weapon.
        /// </summary>
        public Weapon UnarmedWeapon;

        private void Awake()
        {
            // Very important that this happens in Awake as the controller will initialize its inventory systems in the Start method.
            // Dependency - that the slots have an item in it from the designer or else it will fail here.
            RightHand = TryToAssignWeaponInSlotIfExists(Hand.Right, _activeRightHandSlotIndex);
            LeftHand = TryToAssignWeaponInSlotIfExists(Hand.Left, _activeLeftHandSlotIndex);

            _inputHandler = GetComponent<InputHandler>();
            _weaponSocketController = GetComponent<WeaponSocketController>();
        }

        private void Start()
        {
            if (_inputHandler != null) // only playable characters will have one of these
            {
                _inputHandler.OnInputCycleRightHand += InputHandler_CycleRightHand;
                _inputHandler.OnInputCycleLeftHand += InputHandler_CycleLeftHand;
            }
        }

        private void InputHandler_CycleLeftHand()
        {
            CycleWeaponInHand(Hand.Left);
        }

        private void InputHandler_CycleRightHand()
        {
            CycleWeaponInHand(Hand.Right);
        }

        private void CycleWeaponInHand(Hand hand)
        {
            var handSlots = hand == Hand.Right ? RightHandWeaponSlots : LeftHandWeaponSlots;
            
            // these hand slots are kind of like a circular buffer, we have to know when we reach the end and when we do go back to the beginning
            var index = IncrementHandIndexAndReturnValue(hand, handSlots);

            if (hand == Hand.Right)
            {
                RightHand = TryToAssignWeaponInSlotIfExists(hand, index);
                _weaponSocketController.LoadRightHandSocketWithWeapon(RightHand);
            }
            else if (hand == Hand.Left)
            {
                LeftHand = TryToAssignWeaponInSlotIfExists(hand, index);
                _weaponSocketController.LoadLeftHandSocketWithWeapon(LeftHand);
            }
        }

        private int GetHandIndexValue(Hand hand)
            => hand == Hand.Right ? _activeRightHandSlotIndex : _activeLeftHandSlotIndex;

        private int IncrementHandIndexAndReturnValue(Hand hand, Weapon[] handSlots)
        {
            const int oneDimensionalArrayDimension = 0;
            const int beginningOfArray = 0;
            var handIndex = GetHandIndexValue(hand);

            return handIndex == handSlots.GetUpperBound(oneDimensionalArrayDimension) 
                ? SetHandIndexValueAndReturnValue(hand, beginningOfArray) 
                : SetHandIndexValueAndReturnValue(hand, ++handIndex);
        }

        private int SetHandIndexValueAndReturnValue(Hand hand, int newValue)
        {
            const int error = -1;
            if (hand == Hand.Right) _activeRightHandSlotIndex = newValue;
            else if (hand == Hand.Left) _activeLeftHandSlotIndex = newValue;
            else return error;

            return newValue;
        }

        private Weapon TryToAssignWeaponInSlotIfExists(Hand hand, int slotIndex)
        {
            var handSlots = hand == Hand.Right ? RightHandWeaponSlots : LeftHandWeaponSlots;
            return handSlots[slotIndex] != null ? handSlots[slotIndex] : UnarmedWeapon;
        }
    }
}
