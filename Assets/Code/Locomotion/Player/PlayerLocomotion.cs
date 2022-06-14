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

        private Vector3 _targetPosition;

        private PlayerController _playerController;
        private AnimationHandler _animationHandler;
        private Transform _playerTransform;
        private Rigidbody _rigidBody;

        private bool _isRolling;
        private bool _isBackStepping;
        private bool _isSprinting;
        private float _rollButtonPressedTime;
        private bool _rollButtonInvoked;
        private Vector3 _rollDirection;

        private Action _onInteractingAnimationComplete;

        [Header("Stats")] 
        [SerializeField] [Tooltip("How fast the player moves")] private float movementSpeed = 5;
        [SerializeField][Tooltip("How fast the player sprints")] private float sprintSpeed = 8;
        [SerializeField] [Tooltip("How quickly the player can rotate")] private float rotationSpeed = 10; //souls is very fast rotation

        void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _rigidBody = GetComponent<Rigidbody>();
            _animationHandler = GetComponentInChildren<AnimationHandler>();
            _mainCamera = Camera.main.transform;
            _playerTransform = transform;
            _inputHandler = GetComponent<InputHandler>();

            _inputHandler.OnInputRoll += InputHandlerOnInputRoll;
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;

            DecideToRollOrSprint(deltaTime);
            DecideToEndSprint();
            
            //INTERESTING
            //Physics movement is supposed to be put into LateUpdate.  However LateUpdate will not move the character if they are animating
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);
            HandleMovement();

            _animationHandler.UpdateFreelookMovementAnimation(totalMovement, _isSprinting);
            if (_playerController.CanRotate) HandleRotation(deltaTime);
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

        private void HandleMovement()
        {
            var moveDirection = GetXZMoveDirectionFromInput();
            moveDirection *= _isSprinting ? sprintSpeed : movementSpeed;
            _rigidBody.velocity = Vector3.ProjectOnPlane(moveDirection, Vector3.zero);
        }

        private Vector3 GetXZMoveDirectionFromInput()
        {
            var moveDirection = _mainCamera.forward * _inputHandler.MovementInput.y;
            moveDirection += _mainCamera.right * _inputHandler.MovementInput.x;
            moveDirection.Normalize();
            moveDirection.y = 0; //we don't want character moving up or down right now

            return moveDirection;
        }

        private void HandleRotation(float deltaTime)
        {
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

        private void InputHandlerOnInputRoll()
        {
            //When the Roll button is pressed it will either Roll or if no direction was given it will back step the player
            if (_playerController.IsInteracting) return;
            
            var moveDirection = GetXZMoveDirectionFromInput();
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);

            if (totalMovement == 0)
            {
                HandleBackstep();
            }
            else
            {
                _rollDirection = moveDirection;
                _rollButtonPressedTime = 0;
                _rollButtonInvoked = true;
            }
        }

        private void HandleBackstep()
        {
            var moveDirection = _mainCamera.forward * -1;
            //_rigidBody.velocity = moveDirection * movementSpeed;
            _rigidBody.AddForce(moveDirection * movementSpeed, ForceMode.Acceleration);
            _animationHandler.PlayTargetAnimation(AnimationHandler.BACKSTEP_ANIMATION, isInteractingAnimation: true);
            _isBackStepping = true;
            _onInteractingAnimationComplete = FinishBackStep;
        }

        private void DecideToRollOrSprint(float deltaTime)
        {
            if (_isRolling || _isSprinting) return;
            if (!_rollButtonInvoked) return;

            //at this point the roll button had been invoked so we are just waiting to decide to roll or sprint
            if (_inputHandler.RollButtonPressed)
            {
                _rollButtonPressedTime += deltaTime;
                if (_rollButtonPressedTime > _rollButtonPressBeforeSprintInvoked)
                {
                    _isSprinting = true; //handled in the movement code
                    _rollButtonInvoked = false;
                }

                return;
            }

            //no longer pressed, but we didn't trigger a sprint - so roll
            HandleRoll(_rollDirection);
        }

        private void DecideToEndSprint()
        {
            if (!_isSprinting) return;
            if (!_inputHandler.RollButtonPressed) _isSprinting = false;
        }

        private void HandleRoll(Vector3 moveDirection)
        {
            _animationHandler.PlayTargetAnimation(AnimationHandler.ROLLING_ANIMATION, isInteractingAnimation: true);
            var rollRotation = Quaternion.LookRotation(moveDirection);
            _playerTransform.rotation = rollRotation;
            _isRolling = true;
            _onInteractingAnimationComplete = FinishRolling;
        }

        private void FinishRolling()
        {
            _isRolling = false;
            _rollButtonInvoked = false;
            _animationHandler.FinishInteractionAnimation();
        }

        private void FinishBackStep()
        {
            _isBackStepping = false;
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
