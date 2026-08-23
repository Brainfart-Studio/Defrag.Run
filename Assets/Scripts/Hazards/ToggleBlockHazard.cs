using UnityEngine;

public class ToggleBlockHazard : MonoBehaviour
{
    [Tooltip("How long the block stays solid before switching to passthrough")]
    [SerializeField] private float solidDuration = 2f;

    [Tooltip("How long the block stays passthrough before switching to solid")]
    [SerializeField] private float passthroughDuration = 1.5f;

    [SerializeField] private bool startSolid = true;

    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Sprite alpha while passthrough, so the block reads as non-solid at a glance")]
    [Range(0f, 1f)]
    [SerializeField] private float passthroughAlpha = 0.35f;

    private bool isSolid;
    private float phaseTimer;

    private void OnEnable()
    {
        isSolid = startSolid;
        phaseTimer = isSolid ? solidDuration : passthroughDuration;
        ApplyState();
    }

    private void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
        {
            isSolid = !isSolid;
            phaseTimer = isSolid ? solidDuration : passthroughDuration;
            ApplyState();
        }
    }

    private void ApplyState()
    {
        platformCollider.enabled = isSolid;

        Color color = spriteRenderer.color;
        color.a = isSolid ? 1f : passthroughAlpha;
        spriteRenderer.color = color;
    }
}