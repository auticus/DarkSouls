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

        private Vector3 _normalVector;
        private Vector3 _targetPosition;
        private AnimationHandler _animationHandler;

        private Transform _playerTransform;
        private Rigidbody _rigidBody;
        private GameObject _normalCamera;  //free look camera

        [Header("Stats")] [SerializeField] private float MovementSpeed = 5;
        [SerializeField] private float rotationSpeed = 10; //souls is very fast rotation

        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            _animationHandler = GetComponentInChildren<AnimationHandler>();
            _mainCamera = Camera.main.transform;
            _playerTransform = transform;
            _animationHandler.Initialize();
        }

        void Update()
        {
            var deltaTime = Time.deltaTime;
            _inputHandler.Tick(deltaTime);
            HandleMovement();
            _animationHandler.UpdateAnimatorValues(_inputHandler.moveAmount, 0);
            if (_animationHandler.CanRotate()) HandleRotation(deltaTime);
        }

        private void HandleMovement()
        {
            _moveDirection = _mainCamera.forward * _inputHandler.vertical;
            _moveDirection += _mainCamera.right * _inputHandler.horizontal;
            _moveDirection.Normalize();

            _moveDirection *= MovementSpeed;
            _rigidBody.velocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
        }

        private void HandleRotation(float deltaTime)
        {
            var targetVector = Vector3.zero;
            var movementOverride = _inputHandler.moveAmount;

            targetVector = _mainCamera.forward * _inputHandler.vertical;
            targetVector += _mainCamera.right * _inputHandler.horizontal;

            targetVector.Normalize();
            targetVector.y = 0;  //don't care about the y - dont allow it to change

            if (targetVector == Vector3.zero) targetVector = _playerTransform.forward;

            var desiredRotation = Quaternion.LookRotation(targetVector);
            var targetRotation = Quaternion.Slerp(_playerTransform.rotation, desiredRotation, rotationSpeed * deltaTime);

            _playerTransform.rotation = targetRotation;
        }
    }
}
