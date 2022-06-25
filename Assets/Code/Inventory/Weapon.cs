using UnityEngine;

namespace DarkSouls.Inventory
{
    [CreateAssetMenu(menuName = "Items/Weapons")]
    public class Weapon : Item
    {
        public GameObject ModelPrefab;
        public bool IsUnarmed;

        [Header("One Handed Attack Animations")]
        public string OneHandedLightAttack;

        public string OneHandedHeavyAttack;
    }
}
