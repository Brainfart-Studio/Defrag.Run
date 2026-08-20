using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LagZoneHazard : MonoBehaviour
{
    [Tooltip("Multiplier applied to run speed while inside the zone. Lower = slower")]
    [Range(0f, 1f)]
    [SerializeField] private float runSpeedMultiplier = 0.4f;

    [Tooltip("Multiplier applied to fall acceleration and max fall speed while inside the zone. Lower = slower")]
    [Range(0f, 1f)]
    [SerializeField] private float fallSpeedMultiplier = 0.4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player)) return;

        player.SetLagModifiers(runSpeedMultiplier, fallSpeedMultiplier);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player)) return;

        player.ClearLagModifiers();
    }
}