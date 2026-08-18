using BFTools.Core.EventBus;
using BFTools.Feedback.Haptics;
using UnityEngine;

public class DecayProximityHaptics : MonoBehaviour
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

    private void Update()
    {
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

        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
        pulseTimer = Mathf.Lerp(maxPulseInterval, minPulseInterval, t);
    }
}