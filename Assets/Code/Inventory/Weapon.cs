using System.Collections.Generic;
using DarkSouls.Combat;
using UnityEngine;

namespace DarkSouls.Inventory
{
    [CreateAssetMenu(menuName = "Items/Weapons")]
    public class Weapon : Item
    {
        public GameObject ModelPrefab;
        public bool IsUnarmed;
        public int BaseDamage;

        [Header("Animation Details")]
        [field:SerializeField]
        public List<AttackAnimation> LightAttackAnimations = new List<AttackAnimation>();
        
        [field: SerializeField]
        public List<AttackAnimation> HeavyAttackAnimations = new List<AttackAnimation>();

        [Header("Idle Animations")] 
        [field: SerializeField]
        public string RightHandIdle;

        [field: SerializeField] 
        public string LeftHandIdle;

        [Header("Stamina")] 
        [field: SerializeField]
        public int BaseStaminaCost;

        [field: SerializeField] 
        public float HeavyAttackStaminaMultiplier;
    }
}
