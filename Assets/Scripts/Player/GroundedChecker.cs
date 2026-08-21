using BFTools.Core.EventBus;
using UnityEngine;

public struct PlayerLandedEvent
{
    public float FallSpeed;
}

public class GroundedChecker : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    [Tooltip("Minimum downward speed (before landing) required to fire a landing event. Filters out trivial hops/ledge steps")]
    [SerializeField] private float minLandingFallSpeed = 3f;

    private Rigidbody2D rb;
    private bool wasGrounded;
    private float airborneVelocityY;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        IsGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);

        if (IsGrounded && !wasGrounded && airborneVelocityY <= -minLandingFallSpeed)
        {
            EventBus<PlayerLandedEvent>.Fire(new PlayerLandedEvent { FallSpeed = -airborneVelocityY });
        }

        if (!IsGrounded)
        {
            airborneVelocityY = rb.velocity.y;
        }

        wasGrounded = IsGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}