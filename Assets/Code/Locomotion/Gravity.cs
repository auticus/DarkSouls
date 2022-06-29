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
        private const float inTheAirMinimumToLaunchAnimation = 0.5f;
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
            //bitshift 8 and 11 with the tilde means collide against everything except for layer 8 and 11
            _ignoreLayerForGroundCheck = ~(1 << 8 | 1 << 11); //todo: get rid of these magic numbers
        }

        /// <summary>
        /// Handles if the player is falling.
        /// </summary>
        /// <param name="delta"></param>
        public void HandleFalling(float delta, float fallingSpeed, float groundDetectionRayOffset, float groundDetectionRayStartPoint, float minDistanceNeededToBeginFallAnimation,
            Vector3 moveDirection, float movementSpeed, float totalNormalizedMovement, PlayerController playerController)
        {
            /*
             * Something about this method is also responsible for keeping the character above the ground as the collider is currently above its knees
             */
            //Initialization and prep the origin of the ray
            var origin = _characterTransform.position;
            origin.y += groundDetectionRayStartPoint;

            moveDirection = AdjustMovementIfSomethingIsInFront(origin, moveDirection);
            ApplyGravityForceToAerialCharacter(moveDirection, fallingSpeed);
            origin += AdjustGroundDetectRayFrontOrBack(moveDirection, groundDetectionRayOffset); //bump the ray start forward or backward of the player

            var targetPosition = _characterTransform.position; //modifying the position of transform cant happen because its readonly

            //draw a debug ray to help see where this probe is visually
            Debug.DrawRay(origin, -Vector3.up * minDistanceNeededToBeginFallAnimation, Color.red, 0.1f, false);

            var characterIsOnGround = Physics.Raycast(origin,
                -Vector3.up,
                out var groundPoint,
                minDistanceNeededToBeginFallAnimation,
                _ignoreLayerForGroundCheck);

            if (characterIsOnGround) HandleGroundedCharacter(groundPoint, ref targetPosition, playerController);
            else HandleAerialCharacter(movementSpeed);

            LiftCharacterToGroundIfGrounded(totalNormalizedMovement, groundPoint.point);
        }

        private void FinishLanding()
        {
            _animationHandler.FinishInteractionAnimation();
        }

        /// <summary>
        /// When falling or having gravity applied, if there's something right in front of me, we're not moving in any XZ direction.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="moveDirection"></param>
        /// <returns>An adjusted move direction if something is directly in front of the character.</returns>
        private Vector3 AdjustMovementIfSomethingIsInFront(Vector3 origin, Vector3 moveDirection)
        {
            const float raycastInFrontOfPlayerDistance = 0.4f;

            if (Physics.Raycast(origin, _characterTransform.forward, out var hit, raycastInFrontOfPlayerDistance))
            {
                moveDirection = Vector3.zero;
            }

            return moveDirection;
        }

        /// <summary>
        /// Applies a falling speed to a character marked as in the air.
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <param name="fallingSpeed"></param>
        private void ApplyGravityForceToAerialCharacter(Vector3 moveDirection, float fallingSpeed)
        {
            const float ledgeBoostMultipler = 2.0f;
            if (_characterController.IsAerial)
            {
                _rigidBody.AddForce(-Vector3.up * fallingSpeed); //apply the falling speed down (again not realistic should be falling 9.8 m/s extra per turn until terminal velocity)

                //this is supposed to help boost them off ledges and move them forward.  I'm not convinced this is needed
                var movementVelocity = moveDirection * fallingSpeed * ledgeBoostMultipler;

                if (movementVelocity != Vector3.zero)
                {
                    _rigidBody.AddForce(movementVelocity); //allows player to come off ledges or whatever by pushing them forward a tiny bit
                }
            }
        }

        /// <summary>
        /// Adjusts the ground ray front or back of the character.
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <param name="groundDetectionRayOffset"></param>
        /// <returns></returns>
        private Vector3 AdjustGroundDetectRayFrontOrBack(Vector3 moveDirection, float groundDetectionRayOffset)
        {
            var normalizedDirection = moveDirection;
            normalizedDirection.Normalize();
            return moveDirection * groundDetectionRayOffset;
        }

        /// <summary>
        /// Handle characters that are currently identified as being on the ground.
        /// </summary>
        /// <param name="groundPoint"></param>
        /// <param name="targetPosition">A reference to character position.</param>
        /// <param name="onInteractionAnimationComplete"></param>
        private void HandleGroundedCharacter(RaycastHit groundPoint, ref Vector3 targetPosition, PlayerController playerController)
        {
            //this function is what makes the character actually be on the ground instead of sinking up to their knees because their capsule has been
            //lifted up a bit higher

            //we hit the ground, we aren't falling, just put the position down on the ground
            var normalVector = groundPoint.normal; //TODO: keeping this unused because tutorial may use it later but this likely needs killed
            var targetPoint = groundPoint.point;
            _characterController.IsGrounded = true;

            targetPosition.y = targetPoint.y; //this helps bring the character position to the ground (but also needs the lift function as well)
            LandAerialCharacterIfNeeded(playerController);
        }

        /// <summary>
        /// Handle characters that are currently identified as being in the air.
        /// </summary>
        /// <param name="movementSpeed"></param>
        private void HandleAerialCharacter(float movementSpeed)
        {
            //change grounded flag to false if not already and then if not in an aerial state, make some changes to make the aerial state happen
            if (_characterController.IsGrounded)
            {
                _characterController.IsGrounded = false;
            }

            if (_characterController.IsAerial) return;
            
            if (!_characterController.IsInteracting)
            {
                _animationHandler.PlayTargetAnimation(AnimationHandler.FALLING_ANIMATION, isInteractingAnimation: true);
            }

            var velocity = _rigidBody.velocity;
            velocity.Normalize();
            _rigidBody.velocity = velocity * (movementSpeed / 2);
            _characterController.IsAerial = true;
        }

        /// <summary>
        /// Takes the character model and makes sure that they are touching the ground.
        /// </summary>
        /// <param name="totalNormalizedMovement"></param>
        /// <param name="targetPosition"></param>
        private void LiftCharacterToGroundIfGrounded(float totalNormalizedMovement, Vector3 targetPosition)
        {
            if (_characterController.IsGrounded)
            {
                if (_characterController.IsInteracting || totalNormalizedMovement > 0)
                {
                    // EVIL CODE - if you don't divide the delta time by 0.1 the foot will sink into the ground on animations
                    // time.deltatime is a tiny number so dividing it by a fraction actually makes it bigger
                    // the end result here is that the lerp goes faster than if you just use time.deltatime (which will show the foot sinking)
                    _characterTransform.position = Vector3.Lerp(_characterTransform.position, targetPosition,
                        Time.deltaTime / 0.1f);  
                }
                else
                {
                    _characterTransform.position = targetPosition;
                }
            }
        }

        private void LandAerialCharacterIfNeeded(PlayerController playerController)
        {
            if (_characterController.IsAerial)
            {
                //we were in the air but now are about to land
                if (_characterController.AerialTimer > inTheAirMinimumToLaunchAnimation)
                {
                    playerController.OnInteractingAnimationCompleteDoThis = FinishLanding;
                    _animationHandler.PlayTargetAnimation(AnimationHandler.LANDING_ANIMATION, isInteractingAnimation: true);
                }
                else //we weren't in the air long enough to care about a landing animation
                {
                    //without this branch it will sit in a perpetual flying state forever
                    //this takes us to the empty non-state in the animator.
                    _animationHandler.PlayTargetAnimation(AnimationHandler.EMPTY_ANIMATION, isInteractingAnimation: false);
                }
                _characterController.AerialTimer = 0;
                _characterController.IsAerial = false;
            }
        }
    }
}
