using UnityEngine;

namespace DarkSouls.Animation
{
    public class AnimationHandler : MonoBehaviour
    {
        private readonly int _verticalHash = Animator.StringToHash("Vertical");
        private int _horizontalHash = Animator.StringToHash("Horizontal");
        private Animator _animator;
        private bool _canRotate = true;

        private const float ANIMATION_DAMPING_TIME = 0.1f;

        public void Initialize()
        {
            //it appears author did this to force when the Animation Handler would load and not depend on the Start() chain in Unity
            _animator = GetComponent<Animator>();
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            var vertical = GetNormalizedMovement(verticalMovement);
            var horizontal = GetNormalizedMovement(horizontalMovement);
            Debug.Log($"Vertical = {vertical} :: Horizontal ={horizontal}");
            _animator.SetFloat(_verticalHash, vertical, ANIMATION_DAMPING_TIME, Time.deltaTime);
            _animator.SetFloat(_horizontalHash, horizontal, ANIMATION_DAMPING_TIME, Time.deltaTime);
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
