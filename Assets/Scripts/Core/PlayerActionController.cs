using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using ChronoDepths.TimeSystem;

namespace ChronoDepths.Core
{
    /// <summary>
    /// Minimal prototype script that hooks into the <see cref="TimeController"/> by simulating action bursts
    /// whenever the player presses the primary fire button. The goal is to let designers quickly feel the
    /// "semi real-time" pacing described in the design plan without needing a full combat loop yet.
    /// </summary>
    [RequireComponent(typeof(TimeController))]
    public sealed class PlayerActionController : MonoBehaviour
    {
        [SerializeField]
        private float actionDuration = 0.4f;

        [SerializeField]
        private float actionIntensity = 1f;

        private TimeController timeController;
        private bool actionActive;

        private void Awake()
        {
            timeController = GetComponent<TimeController>();
        }

        private void Update()
        {
            if (IsPrimaryFireTriggered())
            {
                TriggerAction();
            }

            if (IsPrimaryFireHeld())
            {
                timeController.NotifyActionPerformed(actionIntensity);
            }
        }

        private void TriggerAction()
        {
            if (actionActive)
            {
                return;
            }

            StartCoroutine(ActionCoroutine());
        }

        private IEnumerator ActionCoroutine()
        {
            actionActive = true;
            timeController.BeginCombatPhase();
            timeController.NotifyActionPerformed(actionIntensity);
            yield return new WaitForSeconds(actionDuration);
            timeController.BeginExplorationPhase();
            actionActive = false;
        }

        private static bool IsPrimaryFireTriggered()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.leftButton.wasPressedThisFrame;
        }

        private static bool IsPrimaryFireHeld()
        {
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.leftButton.isPressed;
        }
    }
}
