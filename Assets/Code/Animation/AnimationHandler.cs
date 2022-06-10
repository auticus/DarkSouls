using DarkSouls.Locomotion.Player;
using UnityEngine;

namespace DarkSouls.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        public const string ROLLING_ANIMATION = "Rolling";
        public const string BACKSTEP_ANIMATION = "Backstep";

        private readonly int _verticalHash = Animator.StringToHash("Vertical");
        private int _horizontalHash = Animator.StringToHash("Horizontal");
        private int _isInteractingHash = Animator.StringToHash("isInteracting");

        private Animator _animator;
        private Rigidbody _playerBody;
        private bool _canRotate = true;
        
        private const float ANIMATION_DAMPING_TIME = 0.1f;
        private const float ANIMATION_CROSSFADE_DAMPING = 0.2f;

        /// <summary>
        /// Gets a value indicating if the animator is in an animation state that cannot be changed until completion.
        /// </summary>
        public bool IsInteracting => _animator.GetBool(_isInteractingHash);

        public void Initialize()
        {
            //it appears author did this to force when the Animation Handler would load and not depend on the Start() chain in Unity
            _animator = GetComponent<Animator>();
            _playerBody = GetComponent<Rigidbody>();
        }

        public void UpdateFreelookMovementAnimation(float totalMovement)
        {
            _animator.SetFloat(_verticalHash, totalMovement, ANIMATION_DAMPING_TIME, Time.deltaTime);
            _animator.SetFloat(_horizontalHash, 0, ANIMATION_DAMPING_TIME, Time.deltaTime);
        }

        /// <summary>
        /// Plays the given animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <param name="isInteractingAnimation">If TRUE will lock the animator down until cleared so that no other movement can interrupt.</param>
        public void PlayTargetAnimation(string animation, bool isInteractingAnimation)
        {
            _animator.applyRootMotion = isInteractingAnimation;
            _animator.SetBool(_isInteractingHash, isInteractingAnimation);
            _animator.CrossFade(animation, ANIMATION_CROSSFADE_DAMPING);
        }

        /// <summary>
        /// Lets the animator know that it is finished interacting and player input can now be honored.
        /// </summary>
        public void FinishInteractionAnimation()
        {
            _animator.SetBool(_isInteractingHash, false);
        }

        public void StartRotation()
        {
            _canRotate = true;
        }

        public void StopRotation()
        {
            //dev note: i'm assuming that we're not using Get or Setter properties because the animator will need to access these functions?
            //if thats not the case, this should be cleaned up and turned into a CanRotate property with get/set
            _canRotate = false;
        }

        /// <summary>
        /// Gets a value indicating if the player in his current animation state may rotate.
        /// </summary>
        /// <returns></returns>
        public bool CanRotate() => _canRotate;

        private void OnAnimatorMove()
        {
            // sync up the camera to the player if they are performing an interactive move
            // as root motion will move the player away from the camera
            // todo: not convinced this is needed with the cinemachine
            if (_animator.GetBool(_isInteractingHash) == false) return;
            var deltaTime = Time.deltaTime;
            _playerBody.drag = 0;

            var deltaPosition = _animator.deltaPosition;
            deltaPosition.y = 0;

            var velocity = deltaPosition / deltaTime;
            _playerBody.velocity = velocity;
        }
    }
}
