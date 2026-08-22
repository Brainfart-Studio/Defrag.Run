using BFTools.Core.EventBus;
using BFTools.Feedback.Haptics;
using BFTools.Feedback.Vignette;
using UnityEngine;

public class DecayProximityFeedback : MonoBehaviour
{
    [Tooltip("Player to measure distance to")]
    [SerializeField] private Transform player;

    [Tooltip("Distance from the decay line at which warning pulses begin")]
    [SerializeField] private float maxDistance = 4f;

    [Tooltip("Distance from the decay line at which pulses reach their fastest rate")]
    [SerializeField] private float minDistance = 1f;

    [Tooltip("Seconds between pulses when the player is at maxDistance")]
    [SerializeField] private float maxPulseInterval = 1f;

    [Tooltip("Seconds between pulses when the player is at or inside minDistance")]
    [SerializeField] private float minPulseInterval = 0.2f;

    private float pulseTimer;
    private bool isPlaying = true;

    private void OnEnable()
    {
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        isPlaying = e.NewState == GameState.Playing;
    }

    private void Update()
    {
        // Stops warning pulses from competing with the Death haptic (and anything
        // after it) once the player is no longer actively playing - this proximity
        // cue only makes sense while there's still a run to warn the player about.
        if (!isPlaying) return;
        if (player == null) return;

        float distance = Mathf.Max(0f, player.position.x - transform.position.x);

        if (distance > maxDistance)
        {
            pulseTimer = 0f;
            return;
        }

        pulseTimer -= Time.deltaTime;
        if (pulseTimer > 0f) return;

        EventBus<BFHapticsEvent>.Fire(new BFHapticsEvent { eventName = "DecayWarning" });
        EventBus<BFVignetteEvent>.Fire(new BFVignetteEvent { eventName = "DecayWarning" });

        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        pulseTimer = Mathf.Lerp(maxPulseInterval, minPulseInterval, t);
    }
}