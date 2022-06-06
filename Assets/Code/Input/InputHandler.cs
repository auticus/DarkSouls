using UnityEngine;

namespace DarkSouls.Input
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        private PlayerControls inputActions;
        private CameraHandler cameraHandler;
        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.thisCameraHandler;
        }

        private void Update()
        {
            //putting rotation in fixedupdate makes things choppy
            //putting it in update makes it super fast
            var deltaTime = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(deltaTime);
                cameraHandler.HandleCameraRotation(deltaTime, mouseX, mouseY);
            }
        }

        private void OnEnable()
        {
            if (inputActions == null) inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void Tick(float deltaTime)
        {
            MoveInput(deltaTime);
        }

        private void MoveInput(float deltaTime)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}