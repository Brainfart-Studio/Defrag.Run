using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Player/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Core Movement")]
    [Tooltip("Horizontal movement speed in units per second")]
    public float runSpeed = 5f;

    [Header("Gravity")]
    [Tooltip("Base gravity strength (negative = downward). Higher magnitude = faster fall")]
    public float gravity = -20f;

    [Tooltip("Maximum downward velocity. Set to 0 to disable clamping. Lower values = slower max fall speed")]
    public float maxFallSpeed = -20f;

    [Tooltip("Linear Drag applied to the Rigidbody when grounded and input is released. Higher value = faster stop.")]
    [Range(0f, 20f)]
    public float groundDrag = 8f;

    [Header("Jump")]
    [Tooltip("Initial upward velocity when jumping. Higher = higher jumps")]
    public float jumpForce = 10f;

    [Tooltip("Time window after jump input to still execute jump. Higher = more lenient")]
    [Range(0f, 0.5f)]
    public float jumpBufferTime = 0.2f;

    [Tooltip("Time after leaving ground where jump is still allowed. Higher = more lenient")]
    [Range(0f, 0.5f)]
    public float coyoteTime = 0.2f;

    [Tooltip("Velocity multiplier when releasing jump early. Lower = faster fall when released")]
    [Range(0f, 1f)]
    public float earlyFallMultiplier = 0.5f;

    [Header("Jump Apex Control")]
    [Tooltip("Gravity multiplier at jump peak. Lower = floatier apex, more hang time")]
    [Range(0f, 1f)]
    public float apexGravityMultiplier = 0.3f;

    [Tooltip("Vertical velocity range to detect apex. Higher = longer apex window")]
    [Range(0f, 2f)]
    public float apexThreshold = 0.1f;

    [Tooltip("Horizontal speed multiplier at apex. Higher = more air control at peak")]
    [Range(1f, 2f)]
    public float apexSpeedMultiplier = 1.2f;

    [Header("Dash")]
    [Tooltip("Horizontal speed applied during dash. Higher = longer/faster dash distance")]
    public float dashSpeed = 20f;

    [Tooltip("Duration of the dash in seconds. Higher = longer dash lockout")]
    [Range(0f, 1f)]
    public float dashDuration = 0.15f;

    [Tooltip("Cooldown after a dash ends before another can be consumed. Higher = longer wait between dashes")]
    [Range(0f, 2f)]
    public float dashCooldown = 0.5f;

    [Tooltip("Number of dashes allowed before landing resets the count. Higher = more air dashes")]
    [Range(1, 5)]
    public int maxDashes = 1;
}