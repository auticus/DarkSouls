using System;
using UnityEngine;

namespace DarkSouls.Combat
{
    /// <summary>
    /// Class that defines an attack animation and any meta data that it has that can affect performance or damage.
    /// </summary>
    [Serializable] //required if you want this to show up in the unity designer
    public class AttackAnimation
    {
        /// <summary>
        /// Gets or sets the name of the animation in the animator.
        /// </summary>
        [field: SerializeField]
        [Tooltip("Gets or sets the name of the animation in the animator.")]
        public string AnimationName { get; set; }

        /// <summary>
        /// Gets or sets the damage multiplier for the animation.
        /// </summary>
        [field: SerializeField]
        [Tooltip("Gets or sets the multiplier for the animation.")]
        public float DamageMultiplier { get; set; } = 1.0f;
    }
}
