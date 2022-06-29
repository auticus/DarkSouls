using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Model class that holds all character state.
    /// </summary>
    public class CharacterState
    {
        private bool _isInteracting;
        private readonly Animator _animator;

        public readonly int IsInteractingHash = Animator.StringToHash("isInteracting");

        public CharacterState(Animator animator)
        {
            _animator = animator;
        }

        /// <summary>
        /// Gets a value indicating if the animator is in an animation state that cannot be changed until completion.
        /// </summary>
        public bool IsInteracting
        {
            get => _isInteracting;
            set
            {
                _animator.SetBool(IsInteractingHash, value);
                _isInteracting = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how long the player has been in the air.
        /// </summary>
        public float AerialTimer { get; set; }

        /// <summary>
        /// Gets or set a value indicating that the player is rolling.
        /// </summary>
        public bool IsRolling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is back stepping.
        /// </summary>
        public bool IsBackStepping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is in the air.
        /// </summary>
        public bool IsAerial { get; set; } //todo: why not just make this IsGrounded = false?

        /// <summary>
        /// Gets or sets a value indicating that the player is performing an attack with their right hand.
        /// </summary>
        public bool IsAttacking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is performing a heavy attack with their right hand.
        /// </summary>
        public bool IsHeavyAttacking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is on the ground.
        /// </summary>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player has been hit by something and is reacting to it via animation.
        /// </summary>
        public bool IsImpacted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is sprinting.
        /// </summary>
        public bool IsSprinting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the roll button was invoked.
        /// </summary>
        public bool RollButtonInvoked { get; set; }

        /// <summary>
        /// Gets a value indicating if the player in his current animation state may rotate.
        /// </summary>
        /// <returns></returns>
        public bool CanRotate { get; set; } = true;
    }
}
