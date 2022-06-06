using UnityEngine;

namespace DarkSouls.Input
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontalMovement;
        public float verticalMovement;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        private PlayerControls inputActions;
        private Vector2 movementInput;
        private Vector2 cameraInput;

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
            //keeping this for now as the calling script can keep the delta time standard across everything
            GetMoveInput(deltaTime);
        }

        private void GetMoveInput(float deltaTime)
        {
            horizontalMovement = movementInput.x;
            verticalMovement = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovement) + Mathf.Abs(verticalMovement));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}