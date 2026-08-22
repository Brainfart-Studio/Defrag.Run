using UnityEngine;

// Dash sets rb.velocity directly at a high constant speed for its duration, and
// Unity's default discrete collision detection can let that punch a step or two
// into solid tile geometry within a single physics step before a collision
// resolves - leaving the player embedded instead of stopped at the surface.
// Rather than let Unity's own depenetration decide a direction (which could just
// as easily shove the player up through the ceiling or down into the kill plane
// as sideways), this forces a single safe direction: right, along the world's
// scroll direction, where there's never solid ground behind the build line.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerUnstuck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("How fast the player is pushed right, in units per second, while embedded in solid tiles")]
    [SerializeField] private float pushSpeed = 8f;

    [Tooltip("Shrinks the overlap check slightly so it doesn't false-positive on tiles the player is only just touching, not actually embedded in")]
    [SerializeField] private float overlapShrink = 0.05f;

    private Collider2D col;
    private Rigidbody2D rb;
    private ContactFilter2D contactFilter;
    private readonly Collider2D[] overlapResults = new Collider2D[4];

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(groundLayer);
        contactFilter.useTriggers = false;
    }

    private void FixedUpdate()
    {
        // Collider is disabled on death (see PlayerController) - nothing to
        // rescue once the run is already over.
        if (!col.enabled) return;

        if (IsEmbedded())
        {
            rb.position += Vector2.right * pushSpeed * Time.fixedDeltaTime;
        }
    }

    private bool IsEmbedded()
    {
        Bounds bounds = col.bounds;
        Vector2 size = (Vector2)bounds.size - Vector2.one * overlapShrink;
        int count = Physics2D.OverlapBox(bounds.center, size, 0f, contactFilter, overlapResults);
        return count > 0;
    }
}