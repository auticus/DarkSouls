using DarkSouls.Animation;
using DarkSouls.Input;
using UnityEngine;

namespace DarkSouls.Locomotion.Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private Transform _mainCamera;
        private InputHandler _inputHandler;
        private Vector3 _moveDirection;

        private Vector3 _targetPosition;
        private AnimationHandler _animationHandler;

        private Transform _playerTransform;
        private Rigidbody _rigidBody;
        private GameObject _normalCamera;  //free look camera

        private bool _isRolling;

        [Header("Stats")] [SerializeField] [Tooltip("How fast the player moves")] private float movementSpeed = 5;
        [SerializeField] [Tooltip("How quickly the player can rotate")] private float rotationSpeed = 10; //souls is very fast rotation

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _animationHandler = GetComponentInChildren<AnimationHandler>();
            _mainCamera = Camera.main.transform;
            _playerTransform = transform;
            _animationHandler.Initialize();
            _inputHandler = GetComponent<InputHandler>();

            _inputHandler.OnInputRoll += InputHandlerOnInputRoll;
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;

            //handle movement used to be here but is physics based

            _animationHandler.UpdateFreelookMovementAnimation(_inputHandler.MovementInput);
            if (_animationHandler.CanRotate()) HandleRotation(deltaTime);
        }

        void FixedUpdate()
        {
            //deviation - HandleMovement is in FixedUpdate here instead of Update because it deals with the rigidbody and physics
            HandleMovement();
        }

        private void HandleMovement()
        {
            _moveDirection = _mainCamera.forward * _inputHandler.MovementInput.y;
            _moveDirection += _mainCamera.right * _inputHandler.MovementInput.x;
            _moveDirection.Normalize();
            _moveDirection.y = 0; //we don't want character moving up or down right now

            _moveDirection *= movementSpeed;
            _rigidBody.velocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.zero);
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
            
        }
    }
}
