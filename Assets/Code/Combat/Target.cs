using System;
using UnityEngine;

namespace DarkSouls.Combat
{
    /// <summary>
    /// Class that represents an object that can be targeted and take damage from attacks.
    /// </summary>
    public class Target : MonoBehaviour
    {
        public event Action<Target> OnDestroyed;
        private PlayerController _character;

        private void Start()
        {
            _character = GetComponent<PlayerController>();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        public void HitTarget(int damage)
        {
            _character.DamageCharacter(damage);
        }
    }
}
