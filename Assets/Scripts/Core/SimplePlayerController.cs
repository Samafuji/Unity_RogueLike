using UnityEngine;
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
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical);

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
            float mouseX = Input.GetAxis("Mouse X");
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
    }
}
