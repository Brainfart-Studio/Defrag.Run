using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Player/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Core Movement")]
    [Tooltip("Horizontal movement speed in units per second")]
    public float runSpeed = 11f;

    [Tooltip("Acceleration toward runSpeed, in units per second squared. Higher = snappier direction changes")]
    public float runAccel = 125f;

    [Tooltip("Multiplier applied to runAccel while airborne. Lower = less air control")]
    [Range(0f, 1f)]
    public float airMult = 0.65f;

    [Header("Gravity")]
    [Tooltip("Base gravity strength (negative = downward). Higher magnitude = faster fall")]
    public float gravity = -112f;

    [Tooltip("Maximum downward velocity. Set to 0 to disable clamping. Lower values = slower max fall speed")]
    public float maxFallSpeed = -20f;

    [Tooltip("Linear Drag applied to the Rigidbody when grounded and input is released. Higher value = faster stop.")]
    [Range(0f, 20f)]
    public float groundDrag = 8f;

    [Header("Jump")]
    [Tooltip("Initial upward velocity when jumping. Higher = higher jumps")]
    public float jumpForce = 13f;

    [Tooltip("Time window after jump input to still execute jump. Higher = more lenient")]
    [Range(0f, 0.5f)]
    public float jumpBufferTime = 0.1f;

    [Tooltip("Time after leaving ground where jump is still allowed. Higher = more lenient")]
    [Range(0f, 0.5f)]
    public float coyoteTime = 0.1f;

    [Tooltip("Time after a jump starts where holding the jump button sustains full launch speed instead of letting gravity pull it down. Releasing early ends the sustain immediately, giving variable jump height")]
    [Range(0f, 0.5f)]
    public float varJumpTime = 0.2f;

    [Header("Jump Apex Control")]
    [Tooltip("Gravity multiplier at jump peak. Lower = floatier apex, more hang time")]
    [Range(0f, 1f)]
    public float apexGravityMultiplier = 0.5f;

    [Tooltip("Vertical velocity range to detect apex. Higher = longer apex window")]
    [Range(0f, 10f)]
    public float apexThreshold = 5f;

    [Tooltip("Horizontal speed multiplier at apex. Higher = more air control at peak")]
    [Range(1f, 2f)]
    public float apexSpeedMultiplier = 1.2f;

    [Header("Dash")]
    [Tooltip("Horizontal speed applied during dash. Higher = longer/faster dash distance")]
    public float dashSpeed = 30f;

    [Tooltip("Duration of the dash in seconds. Higher = longer dash lockout")]
    [Range(0f, 1f)]
    public float dashDuration = 0.15f;

    [Tooltip("Cooldown after a dash ends before another can be consumed. Higher = longer wait between dashes")]
    [Range(0f, 2f)]
    public float dashCooldown = 0.2f;

    [Tooltip("Number of dashes allowed before landing resets the count. Higher = more air dashes")]
    [Range(1, 5)]
    public int maxDashes = 1;
}