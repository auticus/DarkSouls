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
        private Gravity _gravityLocomotion;

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

            _gravityLocomotion = new Gravity(
                _animationHandler,
                _playerController,
                _playerTransform,
                _rigidBody);

            _playerController.IsGrounded = true;
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

            //this is also what keeps the character model up and not sunk in because of the collider being up above the knees
            _gravityLocomotion.HandleFalling(
                deltaTime, 
                fallingSpeed, 
                groundDetectionRayOffset, 
                groundDetectionRayStartPoint,
                minDistanceNeededToBeginFallAnimation, 
                _moveDirection, 
                movementSpeed, 
                GetTotalNormalizedMovement(_inputHandler.MovementInput),
                ref _onInteractingAnimationComplete);
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
