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
        private TimeController timeController;

        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private float turnSpeed = 540f;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 movement = ReadMovementInput();
            Vector3 desiredVelocity = CalculateWorldMoveVector(movement);

            bool hasInput = desiredVelocity.sqrMagnitude > 0.01f;
            if (hasInput)
            {
                Vector3 moveDirection = Vector3.ClampMagnitude(desiredVelocity, 1f);
                characterController.SimpleMove(moveDirection * moveSpeed);
                NotifyAction();
            }
            else
            {
                characterController.SimpleMove(Vector3.zero);
            }

            HandleRotation(desiredVelocity, hasInput);
        }

        private void HandleRotation(Vector3 input, bool hasInput)
        {
            if (hasInput)
            {
                Vector3 forward = new Vector3(input.x, 0f, input.z);
                if (forward.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
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

        private Vector3 CalculateWorldMoveVector(Vector2 movement)
        {
            if (cameraTransform == null)
            {
                return new Vector3(movement.x, 0f, movement.y);
            }

            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            Vector3 world = cameraForward * movement.y + cameraRight * movement.x;
            if (world.sqrMagnitude > 1f)
            {
                world.Normalize();
            }

            return world;
        }
    }
}
