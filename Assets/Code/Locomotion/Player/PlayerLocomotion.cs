using System;
using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace DarkSouls.Locomotion.Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform _mainCamera;
        private InputHandler _inputHandler;
        private readonly float _rollButtonPressBeforeSprintInvoked = 0.5f;
        
        private PlayerController _playerController;
        private CharacterAttributes _playerAttributes; 
        private AnimationHandler _animationHandler;
        private Transform _playerTransform;
        private Rigidbody _rigidBody;

        private float _rollButtonPressedTime;
        private float _jumpingHorizontalForce;
        private Vector3 _interactionDirection;
        private Vector3 _moveDirection;

        private Gravity _gravityLocomotion;

        [Header("Stats")]
        [SerializeField][Tooltip("How fast the player falls")] private float fallingSpeed = 60; //todo: this is not realistic having a constant fall rate
        [SerializeField] [Tooltip("How fast the player moves")] private float movementSpeed = 5;
        [SerializeField][Tooltip("How fast the player sprints")] private float sprintSpeed = 8;
        [SerializeField] [Tooltip("How quickly the player can rotate")] private float rotationSpeed = 10; //souls is very fast rotation

        [Header("Ground & Air Detection Stats")]
        [SerializeField][Tooltip("The point from the player that the raycast begins (the bottom of the collider)")] private float groundDetectionRayStartPoint = 0.5f;
        [SerializeField][Tooltip("Offset point front of player/back of player from where the raycast should begin")] private float groundDetectionRayOffset = 0f;
        [SerializeField][Tooltip("What the distance is before the falling animation should play")] private float minDistanceNeededToBeginFallAnimation = 1f;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _rigidBody = GetComponent<Rigidbody>();
            _animationHandler = GetComponent<AnimationHandler>();
            _playerAttributes = GetComponent<CharacterAttributes>();
            _inputHandler = GetComponent<InputHandler>(); // not optional - this is a player's locomotion
        }

        void Start()
        {
            _mainCamera = Camera.main.transform;
            _playerTransform = transform;
            _inputHandler.OnInputRoll += InputHandler_Roll;
            _inputHandler.OnInputJump += InputHandler_Jump;

            _gravityLocomotion = new Gravity(
                _animationHandler,
                _playerController,
                _playerTransform,
                _rigidBody);

            _playerController.State.IsGrounded = true;
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;

            //INTERESTING
            //Physics movement is supposed to be put into LateUpdate.  However LateUpdate will not move the character if they are animating
            //indicating the movement AND animation logic all need to happen in the same Update or LateUpdate
            _moveDirection = GetXZMoveDirectionFromInput();
            HandleRollingAndSprinting(deltaTime);
            HandleJumping();
            HandleMovement();
            HandleFreeLookAnimations();
            HandleRotation(deltaTime);

            //this is also what keeps the character model up and not sunk in because of the collider being up above the knees
            // if the player is in the middle of a jump (exerting upward force) do not apply gravity until that is finished.
            // this does "defy gravity" a little bit but feels pretty good in the game.  Otherwise with gravity on he doesn't jump as high.
            // alternatively could boost the jump power to add more upwards force but not needed at the moment.
            if (!_playerController.State.IsJumping)
            {
                _gravityLocomotion.HandleFalling(
                    deltaTime,
                    fallingSpeed,
                    groundDetectionRayOffset,
                    groundDetectionRayStartPoint,
                    minDistanceNeededToBeginFallAnimation,
                    _moveDirection,
                    movementSpeed,
                    GetTotalNormalizedMovement(_inputHandler.MovementInput),
                    _playerController);
            }
        }

        /// <summary>
        /// Stops the movement of the character.
        /// </summary>
        public void StopCharacterMovement()
        {
            _rigidBody.velocity = Vector3.zero;
        }

        private void HandleRollingAndSprinting(float deltaTime)
        {
            DecideToRollOrSprint(deltaTime);
            DecideToEndSprint();
        }

        private void HandleMovement()
        {
            if (_playerController.State.IsInteracting || _playerController.State.IsRolling || _playerController.State.IsJumping) return;

            var velocity = _moveDirection * (_playerController.State.IsSprinting ? sprintSpeed : movementSpeed);
            _rigidBody.velocity = Vector3.ProjectOnPlane(velocity, Vector3.zero);
        }

        private void HandleFreeLookAnimations()
        {
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);
            _animationHandler.UpdateFreelookMovementAnimation(totalMovement, _playerController.State.IsSprinting);
        }

        private void HandleRotation(float deltaTime)
        {
            if (!_playerController.State.CanRotate) return;

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

        /// <summary>
        /// Event handler that responds to the roll / sprint button being pressed.
        /// </summary>
        private void InputHandler_Roll()
        {
            //When the Roll button is pressed it will either Roll or if no direction was given it will back step the player
            if (_playerController.State.IsInteracting) return;
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);
            var movementThresholdToSprint = 0.5f;

            if (totalMovement < movementThresholdToSprint)
            {
                HandleBackstep();
            }
            else
            {
                _interactionDirection = GetXZMoveDirectionFromInput();
                _rollButtonPressedTime = 0;
                _playerController.State.RollButtonInvoked = true;
            }
        }

        /// <summary>
        /// Event handler that responds to the jump button being pressed.
        /// </summary>
        private void InputHandler_Jump()
        {
            if (_playerController.State.IsInteracting) return;
            var totalMovement = GetTotalNormalizedMovement(_inputHandler.MovementInput);
            _interactionDirection = GetXZMoveDirectionFromInput();

            InitiateJump(totalMovement);

            // set state and clean up
            _playerController.State.IsJumping = true;
            _playerController.OnInteractingAnimationCompleteDoThis = () =>
            {
                // full attempt at this would be to have animator send an IsJumping false impulse when the animation reached the top of its height 
                // but for now, just let the force be applied to the whole thing and end it when the animator completes full-stop.
                _playerController.State.IsJumping = false;
                _animationHandler.FinishInteractionAnimation();
            };
        }

        private void InitiateJump(float totalMovement)
        {
            // rotate to face the direction you are moving, otherwise if not moving just jump up in place.
            if (totalMovement > 0)
            {
                _animationHandler.PlayTargetAnimation(AnimationHandler.ONE_HANDED_RUN_JUMP, isInteractingAnimation: true);
                var jumpRotation = Quaternion.LookRotation(_interactionDirection);
                _playerTransform.rotation = jumpRotation;

                _jumpingHorizontalForce = _playerController.State.IsSprinting ? sprintSpeed : movementSpeed;
            }
            else
            {
                _animationHandler.PlayTargetAnimation(AnimationHandler.ONE_HANDED_JUMP_IN_PLACE, isInteractingAnimation: true);
                _jumpingHorizontalForce = 0f;
            }
        }

        private void HandleJumping()
        {
            // when jumping the animation has already been triggered by the button press
            // this function is to apply force to the character while they are jumping
            // _interactionDirection is the direction that the player was moving when the jump was initiated.  They are already facing that direction

            if (!_playerController.State.IsJumping) return;

            // apply upward force
            _rigidBody.AddForce(_playerTransform.up * _playerAttributes.Strength);

            // if the player was moving at the time, apply horizontal force\
            // horizontalForce is their move or sprint speed.  
            _rigidBody.AddForce(_interactionDirection * (_jumpingHorizontalForce + _playerAttributes.Strength));
        }

        private void HandleRoll()
        {
            _animationHandler.PlayTargetAnimation(AnimationHandler.ROLLING_ANIMATION, isInteractingAnimation: true);
            var rollRotation = Quaternion.LookRotation(_interactionDirection);
            _playerTransform.rotation = rollRotation;
            _playerController.State.IsRolling = true;
            _playerController.OnInteractingAnimationCompleteDoThis = FinishRolling;
        }

        private void HandleBackstep()
        {
            var moveDirection = _mainCamera.forward * -1;
            //_rigidBody.velocity = moveDirection * movementSpeed;
            _rigidBody.AddForce(moveDirection * movementSpeed, ForceMode.Acceleration);
            _animationHandler.PlayTargetAnimation(AnimationHandler.BACKSTEP_ANIMATION, isInteractingAnimation: true);
            _playerController.State.IsBackStepping = true;
            _playerController.OnInteractingAnimationCompleteDoThis = FinishBackStep;
        }

        private void DecideToRollOrSprint(float deltaTime)
        {
            if (!_playerController.State.RollButtonInvoked) return;
            if (_playerController.State.IsRolling || _playerController.State.IsSprinting) return;

            //at this point the roll button had been invoked so we are just waiting to decide to roll or sprint
            if (_inputHandler.RollButtonPressed)
            {
                _rollButtonPressedTime += deltaTime;
                if (_rollButtonPressedTime > _rollButtonPressBeforeSprintInvoked)
                {
                    _playerController.State.IsSprinting = true; //handled in the movement code
                    _playerController.State.RollButtonInvoked = false;
                }

                return;
            }

            // no longer pressed, but we didn't trigger a sprint - so roll
            // the interaction direction will have been set when the button was initially pressed
            HandleRoll();
        }

        private void DecideToEndSprint()
        {
            if (!_playerController.State.IsSprinting) return;
            if (!_inputHandler.RollButtonPressed) _playerController.State.IsSprinting = false;
        }

        private void FinishRolling()
        {
            _playerController.State.IsRolling = false;
            _playerController.State.RollButtonInvoked = false;
            _animationHandler.FinishInteractionAnimation();
        }

        private void FinishBackStep()
        {
            _playerController.State.IsBackStepping = false;
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
