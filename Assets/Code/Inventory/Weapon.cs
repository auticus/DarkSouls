using UnityEngine;

namespace DarkSouls.Inventory
{
    [CreateAssetMenu(menuName = "Items/Weapons")]
    public class Weapon : Item
    {
        public GameObject ModelPrefab;
        public bool IsUnarmed;
    }
}
