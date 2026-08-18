using BFTools.Core.EventBus;
using UnityEngine;

public struct PlayerDashedEvent { }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementConfig config;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private GroundedChecker groundedChecker;

    private Rigidbody2D rb;
    private float jumpBufferCounter;
    private float coyoteCounter;
    private float varJumpCounter;
    private bool jumpConsumed;
    private int playerDirection = 1;

    private bool isDashing;
    private float dashTimeRemaining;
    private float dashCooldownCounter;
    private int dashesRemaining;

    // Public accessors
    public Rigidbody2D GetRigidbody() => rb;
    public PlayerInputHandler GetInputHandler() => inputHandler;
    public MovementConfig GetConfig() => config;
    public bool IsGrounded => groundedChecker.IsGrounded;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        dashesRemaining = config.maxDashes;
    }

    private void FixedUpdate()
    {
        UpdateJumpTimers();
        UpdateDashTimers();
        ApplyCustomGravity();
        ClampFallSpeed();
        ResetJumpOnLanding();
        ResetDashOnLanding();
        inputHandler.ResetJumpFlag();
        inputHandler.ResetDashFlag();
        UpdateGroundDrag();
    }

    #region Movement
    public void Move(float moveInputX)
    {
        if (isDashing) return;

        float speedMultiplier = GetApexSpeedBoost();
        float targetSpeed = moveInputX * config.runSpeed * speedMultiplier;
        float accel = config.runAccel * (IsGrounded ? 1f : config.airMult);
        float newSpeedX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.velocity = new Vector2(newSpeedX, rb.velocity.y);
        UpdatePlayerDirection(moveInputX);
    }

    private void UpdateGroundDrag()
    {
        if (isDashing)
        {
            rb.drag = 0f;
            return;
        }

        if (IsGrounded && Mathf.Abs(inputHandler.Movement.x) < 0.1f)
        {
            rb.drag = config.groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private float GetApexSpeedBoost()
    {
        return !IsGrounded && IsAtJumpApex() ? config.apexSpeedMultiplier : 1f;
    }

    private void UpdatePlayerDirection(float moveInputX)
    {
        if (moveInputX > 0)
            playerDirection = 1;
        else if (moveInputX < 0)
            playerDirection = -1;

        transform.localScale = new Vector3(playerDirection, 1, 1);
    }
    #endregion

    #region Jump System
    public bool IsJumpInputValid()
    {
        bool hasBufferedJump = config.jumpBufferTime > 0 && jumpBufferCounter > 0;
        bool hasCoyoteTime = config.coyoteTime > 0 && coyoteCounter > 0;
        bool hasJumpInput = hasBufferedJump || inputHandler.JumpedThisFrame;
        bool canJump = IsGrounded || hasCoyoteTime;

        return !jumpConsumed && hasJumpInput && canJump;
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, config.jumpForce);
        varJumpCounter = config.varJumpTime;
    }

    public void ConsumeJump()
    {
        jumpConsumed = true;
        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }

    private void ResetJumpOnLanding()
    {
        if (IsGrounded && rb.velocity.y <= 0)
        {
            jumpConsumed = false;
        }
    }

    private void UpdateJumpTimers()
    {
        UpdateCoyoteTimer();
        UpdateJumpBufferTimer();
    }

    private void UpdateCoyoteTimer()
    {
        if (config.coyoteTime > 0)
        {
            coyoteCounter = IsGrounded ? config.coyoteTime : Mathf.Max(coyoteCounter - Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateJumpBufferTimer()
    {
        if (config.jumpBufferTime > 0)
        {
            if (inputHandler.JumpedThisFrame)
                jumpBufferCounter = config.jumpBufferTime;
            else
                jumpBufferCounter = Mathf.Max(jumpBufferCounter - Time.fixedDeltaTime, 0);
        }
    }

    // While held within varJumpTime of leaving the ground, keeps velocity from dropping
    // below launch speed - gravity is effectively paused rather than scaled, so releasing
    // early cuts the jump short immediately instead of just falling a bit faster.
    public void ApplyVariableJump()
    {
        if (varJumpCounter <= 0f) return;

        if (inputHandler.IsJumpHeld())
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, config.jumpForce));
            varJumpCounter -= Time.fixedDeltaTime;
        }
        else
        {
            varJumpCounter = 0f;
        }
    }
    #endregion

    #region Gravity & Falling
    private void ApplyCustomGravity()
    {
        if (isDashing) return;

        float gravityMultiplier = GetApexGravityReduction();
        rb.velocity += Vector2.down * Mathf.Abs(config.gravity) * gravityMultiplier * Time.fixedDeltaTime;
    }

    private float GetApexGravityReduction()
    {
        return !IsGrounded && IsAtJumpApex() ? config.apexGravityMultiplier : 1f;
    }

    private bool IsAtJumpApex()
    {
        return Mathf.Abs(rb.velocity.y) <= config.apexThreshold;
    }

    public void ClampFallSpeed()
    {
        if (isDashing) return;

        if (config.maxFallSpeed < 0 && rb.velocity.y < config.maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, config.maxFallSpeed);
        }
    }

    public void LimitFallSpeed() => ClampFallSpeed();
    #endregion

    #region Dash System
    public bool IsDashInputValid()
    {
        bool hasDashInput = inputHandler.DashedThisFrame;
        bool canDash = dashesRemaining > 0 && dashCooldownCounter <= 0f && !isDashing;

        return hasDashInput && canDash;
    }

    public void Dash()
    {
        Vector2 direction = inputHandler.Movement.sqrMagnitude > 0.01f
            ? inputHandler.Movement.normalized
            : new Vector2(playerDirection, 0f);

        rb.velocity = direction * config.dashSpeed;
        dashTimeRemaining = config.dashDuration;
        isDashing = true;

        EventBus<PlayerDashedEvent>.Fire(new PlayerDashedEvent());
    }

    public void ConsumeDash()
    {
        dashesRemaining--;
        dashCooldownCounter = config.dashCooldown;
    }

    private void UpdateDashTimers()
    {
        if (isDashing)
        {
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
            }
        }
        else if (dashCooldownCounter > 0f)
        {
            dashCooldownCounter -= Time.fixedDeltaTime;
        }
    }

    private void ResetDashOnLanding()
    {
        if (IsGrounded)
        {
            dashesRemaining = config.maxDashes;
        }
    }
    #endregion
}