using DarkSouls.Input;
using UnityEngine;

namespace DarkSouls
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform; //what we follow
        [SerializeField] private Transform cameraTransform; //the camera itself
        [SerializeField] private Transform cameraPivotTransform; //how the camera turns on a swivel - how it rotates - it rotates around this
        [SerializeField] private float lookSpeed = 0.015f; //dampens the mouse sensitivity left and right
        [SerializeField] private float followSpeed = 0.1f;
        [SerializeField] private float pivotSpeed = 0.015f; //dampens mouse sensitivity up and down
        [SerializeField] private float minimumPivot = -35;
        [SerializeField] private float maximumPivot = 35;

        private Transform _myTransform;
        private InputHandler _inputHandler;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers;  //used for collision
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;

        private void Awake()
        {
            _myTransform = transform;
            _defaultPosition = cameraTransform.localPosition.z;
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); //ignore everything not in 8, 9, or 10? (check out the layers that are created in this proj)

            var gameController = GameObject.FindGameObjectWithTag("GameController");
            _inputHandler = gameController.GetComponent<InputHandler>();
        }

        private void LateUpdate()
        {
            //putting rotation in fixedupdate makes things choppy
            //putting it in update makes it super fast
            var deltaTime = Time.fixedDeltaTime;
            FollowTarget(deltaTime);
            HandleCameraRotation(deltaTime);
        }

        public void FollowTarget(float deltaTime)
        {
            //causes the camera to follow the target transform.position (our player)
            var targetPosition = Vector3.Lerp(_myTransform.position, targetTransform.position, deltaTime / followSpeed);
            _myTransform.position = targetPosition;
        }

        public void HandleCameraRotation(float deltaTime)
        {
            var mouseInputX = _inputHandler.mouseX;
            var mouseInputY = _inputHandler.mouseY;

            _lookAngle += (mouseInputX * lookSpeed) / deltaTime;
            _pivotAngle -= (mouseInputY * pivotSpeed) / deltaTime;
            _pivotAngle = Mathf.Clamp(_pivotAngle, minimumPivot, maximumPivot);

            var rotation = Vector3.zero;
            rotation.y = _lookAngle;
            var targetRotation = Quaternion.Euler(rotation);
            _myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = _pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }
}
