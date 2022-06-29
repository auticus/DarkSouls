using DarkSouls.Characters;
using UnityEngine;

namespace DarkSouls.Combat
{
    /// <summary>
    /// Class representing a dangerous object that can damage players or even other NPCs.
    /// </summary>
    public class DangerousObject : MonoBehaviour
    {
        [SerializeField] [Tooltip("How much damage the object does if touched")] 
        private int damage;

        [SerializeField] [Tooltip("Whether this affects players")]
        private bool affectsPlayers;

        [SerializeField]
        [Tooltip("Whether this affects enemies")]
        private bool affectsEnemies;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!affectsPlayers) return;
            }
            else
            {
                if (!affectsEnemies) return;
            }

            var character = other.gameObject.GetComponent<PlayerController>();
            character.DamageCharacter(damage);
        }
    }
}
