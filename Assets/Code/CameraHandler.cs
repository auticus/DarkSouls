using UnityEngine;

namespace DarkSouls
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform; //what we follow
        [SerializeField] private Transform cameraTransform; //the camera itself
        [SerializeField] private Transform cameraPivotTransform; //how the camera turns on a swivel - how it rotates - it rotates around this
        [SerializeField] private float lookSpeed = 0.1f;
        [SerializeField] private float followSpeed = 0.1f;
        [SerializeField] private float pivotSpeed = 0.03f;
        [SerializeField] private float minimumPivot = -35;
        [SerializeField] private float maximumPivot = 35;

        public static CameraHandler thisCameraHandler;

        private Transform _myTransform;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers;  //used for collision
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;

        private void Awake()
        {
            thisCameraHandler = this;
            _myTransform = transform;
            _defaultPosition = cameraTransform.localPosition.z;
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); //ignore everything not in 8, 9, or 10? (check out the layers that are created in this proj)
        }

        public void FollowTarget(float deltaTime)
        {
            //causes the camera to follow the target transform.position (our player)
            var targetPosition = Vector3.Lerp(_myTransform.position, targetTransform.position, deltaTime / followSpeed);
            _myTransform.position = targetPosition;
        }

        public void HandleCameraRotation(float deltaTime, float mouseInputX, float mouseInputY)
        {
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
