using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CrumbleFallPlatform : MonoBehaviour
{
    [Tooltip("How long the platform shakes in place before it starts falling")]
    [SerializeField] private float shakeDuration = 0.3f;

    [Tooltip("Max random offset applied while shaking, in units")]
    [SerializeField] private float shakeMagnitude = 0.05f;

    [Tooltip("Downward speed once the platform starts falling")]
    [SerializeField] private float fallSpeed = 6f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private enum State { Idle, Shaking, Falling }

    private Rigidbody2D rb;
    private State state;
    private float stateTimer;
    private Vector2 restPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        state = State.Idle;
        stateTimer = 0f;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Shaking:
                stateTimer += Time.fixedDeltaTime;
                ApplyShakeOffset();
                if (stateTimer >= shakeDuration)
                {
                    rb.MovePosition(restPosition);
                    state = State.Falling;
                }
                break;

            case State.Falling:
                rb.MovePosition(rb.position + Vector2.down * fallSpeed * Time.fixedDeltaTime);
                break;
        }
    }

    private void ApplyShakeOffset()
    {
        Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
        rb.MovePosition(restPosition + offset);
    }

    // Player weight starts the shake the instant they land - no grace period, since
    // the camera keeps pushing the player forward regardless. This hazard only ever
    // sits flush in the tile floor, so any player contact means they're standing on
    // it - no need to sanity-check the contact normal direction for that.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != State.Idle) return;
        if (!collision.collider.TryGetComponent<PlayerController>(out _)) return;

        restPosition = rb.position;
        state = State.Shaking;
        stateTimer = 0f;
    }
}