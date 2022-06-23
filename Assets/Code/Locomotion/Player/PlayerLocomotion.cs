using System;
using DarkSouls.Animation;
using DarkSouls.Input;
using UnityEngine;

namespace DarkSouls.Locomotion.Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform _mainCamera;
        private InputHandler _inputHandler;
        private readonly float _rollButtonPressBeforeSprintInvoked = 0.5f;
        
        private PlayerController _playerController;
        private AnimationHandler _animationHandler;
        private Transform _playerTransform;
        private Rigidbody _rigidBody;

        private float _rollButtonPressedTime;
        private Vector3 _rollDirection;
        private Vector3 _moveDirection;

        private Action _onInteractingAnimationComplete;

        private LayerMask _ignoreLayerForGroundCheck;

        [Header("Stats")]
        [SerializeField][Tooltip("How fast the player falls")] private float fallingSpeed = 60; //todo: this is not realistic having a constant fall rate
        [SerializeField] [Tooltip("How fast the player moves")] private float movementSpeed = 5;
        [SerializeField][Tooltip("How fast the player sprints")] private float sprintSpeed = 8;
        [SerializeField] [Tooltip("How quickly the player can rotate")] private float rotationSpeed = 10; //souls is very fast rotation

        [Header("Ground & Air Detection Stats")]
        [SerializeField][Tooltip("The point from the player that the raycast begins (the bottom of the collider)")] private float groundDetectionRayStartPoint = 0.5f;
        [SerializeField][Tooltip("Offset point front of player/back of player from where the raycast should begin")] private float groundDetectionRayOffset = 0.1f;
        [SerializeField][Tooltip("What the distance is before the falling animation should play")] private float minDistanceNeededToBeginFallAnimation = 1f;


        void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _rigidBody = GetComponent<Rigidbody>();
            _animationHandler = GetComponentInChildren<AnimationHandler>();
            _mainCamera = Camera.main.transform;
            _playerTransform = transform;
            _inputHandler = GetComponent<InputHandler>();

            _inputHandler.OnInputRoll += InputHandlerOnInputRoll;

            _playerController.IsGrounded = true;
            _ignoreLayerForGroundCheck = ~(1 << 8 | 1 << 11); //todo: get rid of these magic numbers
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;

            //INTERESTING
            //Physics movement is supposed to be put into LateUpdate.  However LateUpdate will not move the character if they are animating
            //indicating the movement AND animation logic all need to happen in the same Update or LateUpdate
            _moveDirection = GetXZMoveDirectionFromInput();
            HandleRollingAndSprinting(deltaTime);
            HandleMovement();
            HandleFreeLookAnimations();
            HandleRotation(deltaTime);
            HandleFalling(deltaTime); //this is also what keeps the character model up and not sunk in because of the collider being up above the knees
        }

        public void FinishInteractiveAnimation()
        {
            if (_onInteractingAnimationComplete == null)
            {
                Debug.LogError("PlayerLocomotion :: FinishInteractiveAnimation action not set!");
                return;
            }

            _onInteractingAnimationComplete.Invoke();
            _onInteractingAnimationComplete = null;
        }

        private void HandleRollingAndSprinting(float deltaTime)
        {
            DecideToRollOrSprint(deltaTime);
            DecideToEndSprint();
        }

        private void HandleMovement()
        {
            if (_playerController.IsInteracting || _playerController.IsRolling) return;

            var velocity = _moveDirection * (_playerController.IsSprinting ? sprintSpeed : movementSpeed);
            _rigidBody.velocity = Vector3.ProjectOnPlane(velocity, Vector3.zero);
        }

        private void HandleFreeLookAnimations()
        {
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);
            _animationHandler.UpdateFreelookMovementAnimation(totalMovement, _playerController.IsSprinting);
        }

        private void HandleRotation(float deltaTime)
        {
            if (!_playerController.CanRotate) return;

            var targetVector = Vector3.zero;

            targetVector = _mainCamera.forward * _inputHandler.MovementInput.y;
            targetVector += _mainCamera.right * _inputHandler.MovementInput.x;

            targetVector.Normalize();
            targetVector.y = 0;  //don't care about the y - dont allow it to change

            if (targetVector == Vector3.zero) targetVector = _playerTransform.forward;

            var desiredRotation = Quaternion.LookRotation(targetVector);
            var targetRotation = Quaternion.Slerp(_playerTransform.rotation, desiredRotation, rotationSpeed * deltaTime);

            _playerTransform.rotation = targetRotation;
        }

        private void HandleFalling(float delta)
        {
            /*
             * Something about this method is also responsible for keeping the character above the ground as the collider is currently above its knees
             */

            _playerController.IsGrounded = false;

            var origin = _playerTransform.position;
            origin.y += groundDetectionRayStartPoint;
            const float nudgeOffLedgeDivisor = 1f; //the larger this is, the less they will fly forward off of a ledge (since we are dividing by this)
            const float raycastInfrontOfPlayerDistance = 0.4f;

            var moveDirection = _moveDirection;

            if (Physics.Raycast(origin, _playerTransform.forward, out var hit, raycastInfrontOfPlayerDistance))
            {
                //if there's something right in front of me, we're not moving
                moveDirection = Vector3.zero;
            }

            if (_playerController.IsAerial)
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

            var targetPosition = _playerTransform.position;

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
                _playerController.IsGrounded = true;
                targetPosition.y = targetPoint.y;

                if (_playerController.IsAerial)
                {
                    const float inTheAirMinimumToLaunchAnimation = 0.5f;

                    //we were in the air but now are about to land
                    if (_playerController.AerialTimer > inTheAirMinimumToLaunchAnimation)
                    {
                        Debug.Log($"You were in the air for {_playerController.AerialTimer}");
                        _onInteractingAnimationComplete = FinishLanding;
                        _animationHandler.PlayTargetAnimation(AnimationHandler.LANDING_ANIMATION, isInteractingAnimation: true);
                    }
                    else
                    {
                        //_animationHandler.PlayTargetAnimation(AnimationHandler.LOCOMOTION_TREE, isInteractingAnimation: false);
                    }
                    _playerController.AerialTimer = 0;
                    _playerController.IsAerial = false;
                }
            }
            else //we are now flying since the raycast has not detected the ground
            {
                if (_playerController.IsGrounded)
                {
                    _playerController.IsGrounded = false;
                }

                if (!_playerController.IsAerial)
                {
                    if (!_playerController.IsInteracting)
                    {
                        _animationHandler.PlayTargetAnimation(AnimationHandler.FALLING_ANIMATION, isInteractingAnimation: true);
                    }

                    var velocity = _rigidBody.velocity;
                    velocity.Normalize();
                    _rigidBody.velocity = velocity * (movementSpeed / 2);
                    _playerController.IsAerial = true;
                }
            }

            if (_playerController.IsGrounded)
            {
                if (_playerController.IsInteracting || GetTotalNormalizedMovement(_inputHandler.MovementInput) > 0)
                {
                    _playerTransform.position = Vector3.Lerp(_playerTransform.position, targetPosition, Time.deltaTime);
                }
                else
                {
                    _playerTransform.position = targetPosition;
                }
            }
        }

        private Vector3 GetXZMoveDirectionFromInput()
        {
            var moveDirection = _mainCamera.forward * _inputHandler.MovementInput.y;
            moveDirection += _mainCamera.right * _inputHandler.MovementInput.x;
            moveDirection.Normalize();
            moveDirection.y = 0; //we don't want character moving up or down right now

            return moveDirection;
        }

        private void InputHandlerOnInputRoll()
        {
            //When the Roll button is pressed it will either Roll or if no direction was given it will back step the player
            if (_playerController.IsInteracting) return;
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);

            if (totalMovement == 0)
            {
                HandleBackstep();
            }
            else
            {
                _rollDirection = GetXZMoveDirectionFromInput();
                _rollButtonPressedTime = 0;
                _playerController.RollButtonInvoked = true;
            }
        }

        private void HandleBackstep()
        {
            var moveDirection = _mainCamera.forward * -1;
            //_rigidBody.velocity = moveDirection * movementSpeed;
            _rigidBody.AddForce(moveDirection * movementSpeed, ForceMode.Acceleration);
            _animationHandler.PlayTargetAnimation(AnimationHandler.BACKSTEP_ANIMATION, isInteractingAnimation: true);
            _playerController.IsBackStepping = true;
            _onInteractingAnimationComplete = FinishBackStep;
        }

        private void DecideToRollOrSprint(float deltaTime)
        {
            if (_playerController.IsRolling || _playerController.IsSprinting) return;
            if (!_playerController.RollButtonInvoked) return;

            //at this point the roll button had been invoked so we are just waiting to decide to roll or sprint
            if (_inputHandler.RollButtonPressed)
            {
                _rollButtonPressedTime += deltaTime;
                if (_rollButtonPressedTime > _rollButtonPressBeforeSprintInvoked)
                {
                    _playerController.IsSprinting = true; //handled in the movement code
                    _playerController.RollButtonInvoked = false;
                }

                return;
            }

            //no longer pressed, but we didn't trigger a sprint - so roll
            HandleRoll(_rollDirection);
        }

        private void HandleRoll(Vector3 moveDirection)
        {
            _animationHandler.PlayTargetAnimation(AnimationHandler.ROLLING_ANIMATION, isInteractingAnimation: true);
            var rollRotation = Quaternion.LookRotation(moveDirection);
            _playerTransform.rotation = rollRotation;
            _playerController.IsRolling = true;
            _onInteractingAnimationComplete = FinishRolling;
        }

        private void DecideToEndSprint()
        {
            if (!_playerController.IsSprinting) return;
            if (!_inputHandler.RollButtonPressed) _playerController.IsSprinting = false;
        }

        private void FinishRolling()
        {
            _playerController.IsRolling = false;
            _playerController.RollButtonInvoked = false;
            _animationHandler.FinishInteractionAnimation();
        }

        private void FinishBackStep()
        {
            _playerController.IsBackStepping = false;
            _animationHandler.FinishInteractionAnimation();
        }

        private void FinishLanding()
        {
            _animationHandler.FinishInteractionAnimation();
        }

        /// <summary>
        /// Given a movementInput <see cref="Vector2"/>, calculate the absolute total normalized movement of both the X and Y values.
        /// Normalized Movement returns floats from 0, 0.5, and 1.
        /// </summary>
        /// <param name="movementInput">The Vector2 movement input from the controller.</param>
        /// <returns>A value indicating the total movement.</returns>
        private static float GetTotalNormalizedMovement(Vector2 movementInput)
        {
            // examine the x and y movement and add them together and pass that sum as the vertical value to the animator
            // horizontal for free look will be currently hard coded to zero

            var vertical = Mathf.Abs(GetNormalizedMovement(movementInput.y));
            var horizontal = Mathf.Abs(GetNormalizedMovement(movementInput.x));

            return Mathf.Clamp01(vertical + horizontal);
        }

        private static float GetNormalizedMovement(float movement)
        {
            if (movement > 0 && movement <= 0.55f) return 0.5f;
            if (movement > 0.55f) return 1f;
            if (movement < 0 && movement > -0.55f) return -0.5f;
            if (movement < -0.55f) return -1f;

            return 0;
        }
    }
}
