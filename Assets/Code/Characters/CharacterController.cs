using UnityEngine;

namespace DarkSouls.Characters
{
    public class CharacterController : MonoBehaviour, ICharacterController
    {
        public void DamageCharacter(int damage)
        {
            throw new System.NotImplementedException();
        }

        public void EnableAttackingWeaponCollider()
        {
            Debug.LogError("CharacterController::EnableAttackingWeaponCollider not implemented");    
        }

        public void DisableAttackingWeaponCollider()
        {
            Debug.LogError("CharacterController::DisableAttackingWeaponCollider not implemented");
        }
    }
}
