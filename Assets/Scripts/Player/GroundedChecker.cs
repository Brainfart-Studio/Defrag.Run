using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    private void FixedUpdate()
    {
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        IsGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}