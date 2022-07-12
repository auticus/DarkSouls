using DarkSouls.Characters;
using UnityEngine;

namespace DarkSouls.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        public const string EMPTY_ANIMATION = "Empty";
        public const string ROLLING_ANIMATION = "Rolling";
        public const string BACKSTEP_ANIMATION = "Backstep";
        public const string LANDING_ANIMATION = "Landing";
        public const string FALLING_ANIMATION = "Falling";
        public const string ONE_HANDED_IMPACT_FRONT_STEPBACK_01 = "oh_hit_front_stepback_01";
        public const string ONE_HANDED_DEATH_01 = "oh_death_01";
        public const string RIGHT_ARM_IDLE_EMPTY = "Right Arm Empty";
        public const string LEFT_ARM_IDLE_EMPTY = "Left Arm Empty";
        public const string PICKUP_ITEM = "Pick up Item";
        public const string ONE_HANDED_JUMP_IN_PLACE = "oh_Jump";
        public const string ONE_HANDED_RUN_JUMP = "oh_Run_Jump";

        public LocomotionStates CurrentLocomotionState = LocomotionStates.FreeLook;

        private readonly int _targetingBlendTree = Animator.StringToHash("TargetingBlendTree");
        private readonly int _freeLookBlendTree = Animator.StringToHash("FreeLookBlendTree");
        private readonly int _verticalHash = Animator.StringToHash("Vertical");
        private readonly int _horizontalHash = Animator.StringToHash("Horizontal");

        private PlayerController _playerController;
        private Animator _animator;
        private Rigidbody _playerBody;

        private const float ANIMATION_DAMPING_TIME = 0.1f;
        private const float ANIMATION_CROSSFADE_DAMPING = 0.2f;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _playerBody = transform.GetComponent<Rigidbody>();
            _playerController = transform.GetComponent<PlayerController>();
        }

        private void OnAnimatorMove()
        {
            //this helps sync animations so that they do not sink into the ground
            //otherwise your rolling or whatever animations will partially submerge into the ground and look stupid.

            if (_playerController.State.IsInteracting == false) return;
            var deltaTime = Time.deltaTime;
            _playerBody.drag = 0;

            var deltaPosition = _animator.deltaPosition;
            deltaPosition.y = 0;

            var velocity = deltaPosition / deltaTime;
            _playerBody.velocity = velocity;

        }

        /// <summary>
        /// Updates the Animation flags based on the total movement indicated by the player input.
        /// </summary>
        /// <param name="totalMovement">The combined values of the x and y axis.</param>
        /// <param name="isSprinting">A flag indicating if the player is sprinting or not.</param>
        public void UpdateFreelookMovementAnimation(float totalMovement, bool isSprinting, float deltaTime)
        {
            // the vertical hash controls the animation for walking or sprinting.  a value of 0-1 blends the walk whereas a 2 is full on sprint.
            const float sprintAnimationValue = 2.0f;
            
            if (isSprinting) totalMovement = sprintAnimationValue;
            _animator.SetFloat(_verticalHash, totalMovement, ANIMATION_DAMPING_TIME, deltaTime);
            _animator.SetFloat(_horizontalHash, 0, ANIMATION_DAMPING_TIME, deltaTime);
        }

        /// <summary>
        /// Updates the Animation flags based on the x and y input values.
        /// </summary>
        /// <param name="x">The player X input on the keyboard/gamepad.</param>
        /// <param name="y">The player Y input on the keyboard/gamepad.</param>
        /// <param name="isSprinting">A flag indicating if the player is sprinting or not.</param>
        public void UpdateTargetingMovementAnimation(float x, float y, bool isSprinting, float deltaTime)
        {
            const float sprintAnimationValue = 1.0f;
            if (isSprinting)
            {
                x += sprintAnimationValue;
                y += sprintAnimationValue;
            }
            
            _animator.SetFloat(_verticalHash, y, ANIMATION_DAMPING_TIME, deltaTime);
            _animator.SetFloat(_horizontalHash, x, ANIMATION_DAMPING_TIME, deltaTime);
        }

        /// <summary>
        /// Plays the given animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <param name="isInteractingAnimation">If TRUE will lock the animator down until cleared so that no other movement can interrupt.</param>
        public void PlayTargetAnimation(string animation, bool isInteractingAnimation)
        {
            _animator.applyRootMotion = isInteractingAnimation;
            _playerController.State.IsInteracting = isInteractingAnimation;
            _animator.CrossFade(animation, ANIMATION_CROSSFADE_DAMPING);
        }

        /// <summary>
        /// Transitions blend trees to another locomotion state.
        /// </summary>
        public void TransitionLocomotionState(LocomotionStates locomotionState)
        {
            _animator.CrossFadeInFixedTime(
                locomotionState == LocomotionStates.FreeLook 
                    ? _freeLookBlendTree 
                    : _targetingBlendTree,
                ANIMATION_CROSSFADE_DAMPING);

            CurrentLocomotionState = locomotionState;
        }

        /// <summary>
        /// Lets the animator know that it is finished interacting and player input can now be honored.
        /// </summary>
        public void FinishInteractionAnimation()
        {
            _playerController.State.IsInteracting = false;
            _playerController.State.IsAbleToCombo = false;
        }
    }
}
