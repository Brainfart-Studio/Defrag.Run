using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class LagZoneTrigger : MonoBehaviour
{
    [Tooltip("Multiplier applied to run speed while inside the zone. Lower = slower")]
    [Range(0f, 1f)]
    [SerializeField] private float runSpeedMultiplier = 0.4f;

    [Tooltip("Multiplier applied to fall acceleration and max fall speed while inside the zone. Lower = slower")]
    [Range(0f, 1f)]
    [SerializeField] private float fallSpeedMultiplier = 0.4f;

    private PlayerController currentPlayer;
    private Coroutine pendingClear;

    // One shared composite collider for the whole tilemap, so cell-to-cell movement
    // inside the zone never re-fires enter/exit - only crossing the outer edge does.
    // Building a new column while the player is already standing on an earlier one
    // still rebuilds the composite fixture though, which can cost a one-frame false
    // exit/enter pair - the delayed clear below absorbs that instead of it reading
    // as a real exit.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player)) return;

        if (pendingClear != null)
        {
            StopCoroutine(pendingClear);
            pendingClear = null;
        }

        currentPlayer = player;
        player.SetLagModifiers(runSpeedMultiplier, fallSpeedMultiplier);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player)) return;
        if (player != currentPlayer) return;

        pendingClear = StartCoroutine(ClearAfterDelay(player));
    }

    private IEnumerator ClearAfterDelay(PlayerController player)
    {
        yield return new WaitForFixedUpdate();

        pendingClear = null;
        currentPlayer = null;
        player.ClearLagModifiers();
    }
}