using System;
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
        [SerializeField] private float cameraSphereRadius = 0.2f;
        [SerializeField] private float cameraCollisionOffset = 0.2f;
        [SerializeField] private float minimumCollisionOffset = 0.2f;
        [SerializeField] private float cameraCollisionCorrectionDampening = 0.2f;

        private Transform _myTransform;
        private float _targetPosition;
        private InputHandler _inputHandler;
        private Vector3 _cameraTransformPosition;
        private Vector3 _cameraFollowVelocity;
        private LayerMask _ignoreLayers;  //used for collision
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;

        private void Awake()
        {
            _myTransform = transform;
            _defaultPosition = cameraTransform.localPosition.z;
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); //ignore everything in 8, 9, or 10? (check out the layers that are created in this proj)
            //Camera rig and pivot are in layer 10 - Controller as is the player

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
            
            //OLD CODE for reference on how not to do things
            //var targetPosition = Vector3.Lerp(_myTransform.position, targetTransform.position, deltaTime / followSpeed);

            //Use smoothDamp over Lerp to remove jittery (docs confirm this is the right way to go as well)
            var targetPosition = Vector3.SmoothDamp(
                _myTransform.position,
                targetTransform.position,
                ref _cameraFollowVelocity,
                deltaTime / followSpeed);

            _myTransform.position = targetPosition;
            HandleCameraCollisions(deltaTime);
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

        private void HandleCameraCollisions(float deltaTime)
        {
            //Current issue with this is that while it does work some of the time, often it just wants to zoom in super close to the character
            //which is not good.  Needs to limit how far it wants to zoom in

            _targetPosition = _defaultPosition;
            var direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            //Casts a sphere along a ray and returns info on what was hit.  Useful if you want to find out if an object of a specific zie, such as a character,
            //will be able to move somewhere without colliding with anything on the way.  Its like a "thick raycast".
            if (Physics.SphereCast(
                    cameraPivotTransform.position, 
                    cameraSphereRadius, 
                    direction, 
                    out var hit,
                    Mathf.Abs(_targetPosition), 
                    _ignoreLayers))
            {
                var distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                _targetPosition = -(distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(_targetPosition) < minimumCollisionOffset)
            {
                _targetPosition = -minimumCollisionOffset;
            }

            //once again should use smooth damp instead of lerp
            _cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, _targetPosition,
                deltaTime / cameraCollisionCorrectionDampening);
            cameraTransform.localPosition = _cameraTransformPosition;
        }
    }
}
