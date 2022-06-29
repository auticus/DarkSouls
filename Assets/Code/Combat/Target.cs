﻿using DarkSouls.Characters;
using UnityEngine;

namespace DarkSouls.Combat
{
    /// <summary>
    /// Class that represents an object that can be targeted and take damage from attacks.
    /// </summary>
    public class Target : MonoBehaviour
    {
        private PlayerController _character;

        private void Start()
        {
            _character = GetComponent<PlayerController>();
        }

        public void HitTarget(int damage)
        {
            _character.DamageCharacter(damage);
        }
    }
}