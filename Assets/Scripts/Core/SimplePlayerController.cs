using UnityEngine;
using UnityEngine.InputSystem;
using ChronoDepths.TimeSystem;

namespace ChronoDepths.Core
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class SimplePlayerController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 6f;

        [SerializeField]
        private float turnSpeed = 540f;

        [SerializeField]
        private TimeController timeController;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 movement = ReadMovementInput();
            Vector3 input = new Vector3(movement.x, 0f, movement.y);

            bool hasInput = input.sqrMagnitude > 0.01f;
            if (hasInput)
            {
                input = Vector3.ClampMagnitude(input, 1f);
                Vector3 moveDirection = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * input;
                characterController.SimpleMove(moveDirection * moveSpeed);
                NotifyAction();
            }
            else
            {
                characterController.SimpleMove(Vector3.zero);
            }

            HandleRotation(input, hasInput);
        }

        private void HandleRotation(Vector3 input, bool hasInput)
        {
            float mouseX = ReadMouseDeltaX();
            if (Mathf.Abs(mouseX) > 0.01f)
            {
                transform.Rotate(Vector3.up, mouseX * turnSpeed * Time.deltaTime);
                NotifyAction(Mathf.Clamp01(Mathf.Abs(mouseX)));
            }
            else if (hasInput)
            {
                Vector3 forward = new Vector3(input.x, 0f, input.z);
                if (forward.sqrMagnitude > 0.001f)
                {
                    Vector3 worldForward = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * forward;
                    Quaternion targetRotation = Quaternion.LookRotation(worldForward, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                }
            }
        }

        private void NotifyAction(float intensity = 1f)
        {
            if (timeController != null)
            {
                timeController.NotifyActionPerformed(intensity);
            }
        }

        private static Vector2 ReadMovementInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            Vector2 movement = Vector2.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                movement.y += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                movement.y -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                movement.x += 1f;
            }

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                movement.x -= 1f;
            }

            if (movement.sqrMagnitude > 1f)
            {
                movement = movement.normalized;
            }

            return movement;
        }

        private static float ReadMouseDeltaX()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return 0f;
            }

            Vector2 delta = mouse.delta.ReadValue();
            return delta.x;
        }
    }
}
