using UnityEngine;
using UnityEngine.InputSystem;

namespace ChronoDepths.Core
{
    public sealed class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 focusOffset = new Vector3(0f, 1.5f, 0f);

        [SerializeField]
        private float distance = 6f;

        [SerializeField]
        private float minPitch = 10f;

        [SerializeField]
        private float maxPitch = 60f;

        [SerializeField]
        private float yawSensitivity = 120f;

        [SerializeField]
        private float pitchSensitivity = 90f;

        [SerializeField]
        private float followSharpness = 12f;

        [SerializeField]
        private bool autoTargetPlayer = true;

        private float yaw;

        private float pitch;

        private Vector3 focusPosition;

        private void OnEnable()
        {
            InitialiseRig();
        }

        private void InitialiseRig()
        {
            if (target == null && autoTargetPlayer)
            {
                SimplePlayerController player = FindObjectOfType<SimplePlayerController>();
                if (player != null)
                {
                    SetTarget(player.transform);
                }
            }

            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
            if (pitch > 180f)
            {
                pitch -= 360f;
            }
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            if (target != null)
            {
                focusPosition = target.position + focusOffset;
            }
            else
            {
                focusPosition = transform.position;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector2 lookDelta = ReadLookInput();
            yaw += lookDelta.x * yawSensitivity * Time.deltaTime;
            pitch -= lookDelta.y * pitchSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Vector3 targetFocus = target.position + focusOffset;
            float interpolation = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
            focusPosition = Vector3.Lerp(focusPosition, targetFocus, interpolation);

            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = focusPosition + orbitRotation * new Vector3(0f, 0f, -distance);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, interpolation);
            transform.rotation = Quaternion.LookRotation(focusPosition - transform.position, Vector3.up);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                focusPosition = target.position + focusOffset;
            }
        }

        private static Vector2 ReadLookInput()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return Vector2.zero;
            }

            return mouse.delta.ReadValue();
        }
    }
}
