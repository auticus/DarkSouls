using UnityEngine;

namespace DarkSouls.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        private readonly int _verticalHash = Animator.StringToHash("Vertical");
        private int _horizontalHash = Animator.StringToHash("Horizontal");
        private int _isInteractingHash = Animator.StringToHash("isInteracting");

        private Animator _animator;
        private bool _canRotate = true;
        
        private const float ANIMATION_DAMPING_TIME = 0.1f;
        private const float ANIMATION_CROSSFADE_DAMPING = 0.2f;

        public void Initialize()
        {
            //it appears author did this to force when the Animation Handler would load and not depend on the Start() chain in Unity
            _animator = GetComponent<Animator>();
        }

        public void UpdateFreelookMovementAnimation(Vector2 movementInput)
        {
            // examine the x and y movement and add them together and pass that sum as the vertical value to the animator
            // horizontal for free look will be currently hard coded to zero

            var vertical = Mathf.Abs(GetNormalizedMovement(movementInput.y));
            var horizontal = Mathf.Abs(GetNormalizedMovement(movementInput.x));

            var totalMovement = Mathf.Clamp01(vertical + horizontal);
            _animator.SetFloat(_verticalHash, totalMovement, ANIMATION_DAMPING_TIME, Time.deltaTime);
            _animator.SetFloat(_horizontalHash, 0, ANIMATION_DAMPING_TIME, Time.deltaTime);
        }

        /// <summary>
        /// Plays the given animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <param name="isInteracting">If TRUE will lock the animator down until cleared so that no other movement can interrupt.</param>
        public void PlayTargetAnimation(string animation, bool isInteracting)
        {
            _animator.applyRootMotion = isInteracting;
            _animator.SetBool(_isInteractingHash, isInteracting);
            _animator.CrossFade(animation, ANIMATION_CROSSFADE_DAMPING);
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

        private float GetNormalizedMovement(float movement)
        {
            if (movement > 0 && movement <= 0.55f) return 0.5f;
            if (movement > 0.55f) return 1f;
            if (movement < 0 && movement > -0.55f) return -0.5f;
            if (movement < -0.55f) return -1f;

            return 0;
        }
    }
}
