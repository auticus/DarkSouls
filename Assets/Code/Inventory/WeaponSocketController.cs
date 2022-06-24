using UnityEngine;

namespace DarkSouls.Inventory
{
    /// <summary>
    /// Controller class that will manage what is in the player's hands.
    /// </summary>
    public class WeaponSocketController : MonoBehaviour
    {
        private WeaponSocket _leftHand;
        private WeaponSocket _rightHand;

        private void Awake()
        {
            var sockets = GetComponentsInChildren<WeaponSocket>();
            foreach (var socket in sockets)
            {
                if (socket.LeftHandSocket) _leftHand = socket;
                else if (socket.RightHandSocket) _rightHand = socket;
            }
        }

        public void LoadRightHandSocketWithWeapon(Weapon weapon)
        {
            _leftHand.LoadWeaponModel(weapon);
        }

        public void LoadLeftHandSocketWithWeapon(Weapon weapon)
        {
            _rightHand.LoadWeaponModel(weapon);
        }
    }
}
