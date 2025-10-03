using UnityEngine;

namespace ChronoDepths.TimeSystem
{
    /// <summary>
    /// Controls the game's time scale based on the player's activity. When an action is in progress
    /// the time scale speeds up towards <see cref="actionTimeScale"/> and when idle it eases back towards
    /// <see cref="idleTimeScale"/>. Other systems can subscribe to <see cref="OnTimeScaleChanged"/> to
    /// react (e.g. adjusting audio pitch or shader effects).
    /// </summary>
    public sealed class TimeController : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 1f)]
        private float idleTimeScale = 0.1f;

        [SerializeField, Range(0.1f, 4f)]
        private float actionTimeScale = 1.5f;

        [SerializeField, Tooltip("How quickly the timescale interpolates towards the target values.")]
        private float interpolationSpeed = 5f;

        [SerializeField, Tooltip("Debug toggle that prevents the system from modifying Time.timeScale.")]
        private bool previewOnly;

        private float targetScale = 1f;
        private float currentScale = 1f;
        private float timeSinceLastAction;

        public event System.Action<float> OnTimeScaleChanged;

        public float IdleTimeScale => idleTimeScale;
        public float ActionTimeScale => actionTimeScale;
        public float CurrentScale => currentScale;
        public bool HasRecentAction => timeSinceLastAction < 0.3f;

        private void OnEnable()
        {
            currentScale = idleTimeScale;
            ApplyTimeScale(currentScale, force: true);
        }

        private void Update()
        {
            timeSinceLastAction += Time.unscaledDeltaTime;
            targetScale = HasRecentAction ? actionTimeScale : idleTimeScale;
            currentScale = Mathf.MoveTowards(currentScale, targetScale, interpolationSpeed * Time.unscaledDeltaTime);
            ApplyTimeScale(currentScale, force: false);
        }

        public void NotifyActionPerformed(float intensity = 1f)
        {
            intensity = Mathf.Clamp01(intensity);
            timeSinceLastAction = Mathf.Lerp(0f, timeSinceLastAction, 1f - intensity);
            targetScale = Mathf.Lerp(idleTimeScale, actionTimeScale, intensity);
            ApplyTimeScale(targetScale, force: false);
        }

        public void BeginExplorationPhase()
        {
            timeSinceLastAction = float.MaxValue;
        }

        public void BeginCombatPhase()
        {
            timeSinceLastAction = 0f;
            targetScale = actionTimeScale;
        }

        private void ApplyTimeScale(float scale, bool force)
        {
            if (!force && Mathf.Approximately(Time.timeScale, scale))
            {
                return;
            }

            if (!previewOnly)
            {
                Time.timeScale = scale;
            }

            OnTimeScaleChanged?.Invoke(scale);
        }
    }
}
