using System;
using DarkSouls.Animation;
using UnityEngine;

namespace DarkSouls.Locomotion
{
    /// <summary>
    /// Helper class that is used to handle falling and any gravity type locomotion.
    /// </summary>
    public class Gravity
    {
        private readonly AnimationHandler _animationHandler;
        private readonly PlayerController _characterController;
        private readonly Transform _characterTransform;
        private readonly LayerMask _ignoreLayerForGroundCheck;
        private readonly Rigidbody _rigidBody;

        public Gravity(AnimationHandler animationHandler, 
            PlayerController characterController, 
            Transform characterTransform,
            Rigidbody characterRigidBody)
        {
            _animationHandler = animationHandler;
            _characterController = characterController;
            _characterTransform = characterTransform;
            _rigidBody = characterRigidBody;
            _ignoreLayerForGroundCheck = ~(1 << 8 | 1 << 11); //todo: get rid of these magic numbers
        }

        /// <summary>
        /// Handles if the player is falling.
        /// </summary>
        /// <param name="delta"></param>
        public void HandleFalling(float delta, float fallingSpeed, float groundDetectionRayOffset, float groundDetectionRayStartPoint, float minDistanceNeededToBeginFallAnimation,
            Vector3 moveDirection, float movementSpeed, float totalNormalizedMovement, ref Action onInteractionAnimationComplete)
        {
            /*
             * Something about this method is also responsible for keeping the character above the ground as the collider is currently above its knees
             */

            _characterController.IsGrounded = false;

            var origin = _characterTransform.position;
            origin.y += groundDetectionRayStartPoint;
            const float nudgeOffLedgeDivisor = 1f; //the larger this is, the less they will fly forward off of a ledge (since we are dividing by this)
            const float raycastInFrontOfPlayerDistance = 0.4f;

            if (Physics.Raycast(origin, _characterTransform.forward, out var hit, raycastInFrontOfPlayerDistance))
            {
                //if there's something right in front of me, we're not moving
                moveDirection = Vector3.zero;
            }

            if (_characterController.IsAerial)
            {
                _rigidBody.AddForce(-Vector3.up * fallingSpeed); //apply the falling speed down (again not realistic should be falling 9.8 m/s extra per turn until terminal velocity)

                //this is supposed to help boost them off ledges and move them forward.  I'm not convinced this is needed
                var movementVelocity = moveDirection * fallingSpeed / nudgeOffLedgeDivisor;

                if (movementVelocity != Vector3.zero)
                {
                    _rigidBody.AddForce(moveDirection * fallingSpeed / nudgeOffLedgeDivisor, ForceMode.Acceleration); //allows player to come off ledges or whatever by pushing them forward a tiny bit
                }
            }

            var direction = moveDirection;
            direction.Normalize();
            origin = origin + direction * groundDetectionRayOffset; //bump the ray start forward or backward of the player

            var targetPosition = _characterTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minDistanceNeededToBeginFallAnimation, Color.red, 0.1f, false);
            if (Physics.Raycast(origin,
                    -Vector3.up,
                    out hit,
                    minDistanceNeededToBeginFallAnimation,
                    _ignoreLayerForGroundCheck))
            {
                //we hit the ground, we aren't falling, just put the position down on the ground
                var normalVector = hit.normal;
                var targetPoint = hit.point;
                _characterController.IsGrounded = true;
                targetPosition.y = targetPoint.y;

                if (_characterController.IsAerial)
                {
                    const float inTheAirMinimumToLaunchAnimation = 0.5f;

                    //we were in the air but now are about to land
                    if (_characterController.AerialTimer > inTheAirMinimumToLaunchAnimation)
                    {
                        Debug.Log($"You were in the air for {_characterController.AerialTimer}");
                        onInteractionAnimationComplete = FinishLanding;
                        _animationHandler.PlayTargetAnimation(AnimationHandler.LANDING_ANIMATION, isInteractingAnimation: true);
                    }
                    else
                    {
                        //_animationHandler.PlayTargetAnimation(AnimationHandler.LOCOMOTION_TREE, isInteractingAnimation: false);
                    }
                    _characterController.AerialTimer = 0;
                    _characterController.IsAerial = false;
                }
            }
            else //we are now flying since the raycast has not detected the ground
            {
                if (_characterController.IsGrounded)
                {
                    _characterController.IsGrounded = false;
                }

                if (!_characterController.IsAerial)
                {
                    if (!_characterController.IsInteracting)
                    {
                        _animationHandler.PlayTargetAnimation(AnimationHandler.FALLING_ANIMATION, isInteractingAnimation: true);
                    }

                    var velocity = _rigidBody.velocity;
                    velocity.Normalize();
                    _rigidBody.velocity = velocity * (movementSpeed / 2);
                    _characterController.IsAerial = true;
                }
            }

            if (_characterController.IsGrounded)
            {
                if (_characterController.IsInteracting || totalNormalizedMovement > 0)
                {
                    _characterTransform.position = Vector3.Lerp(_characterTransform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    _characterTransform.position = targetPosition;
                }
            }
        }

        private void FinishLanding()
        {
            _animationHandler.FinishInteractionAnimation();
        }
    }
}
