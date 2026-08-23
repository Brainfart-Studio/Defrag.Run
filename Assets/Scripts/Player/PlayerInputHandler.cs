using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private TouchJoystick touchJoystick;
    [SerializeField] private TouchButton touchJumpButton;
    [SerializeField] private TouchButton touchDashButton;

    private InputAction move;
    private InputAction jump;
    private InputAction dash;

    public Vector2 Movement { get; private set; }
    public bool JumpedThisFrame { get; private set; }
    public bool DashedThisFrame { get; private set; }

    private void Start()
    {
        move = InputManager.Instance.GetGameplayAction("Move");
        jump = InputManager.Instance.GetGameplayAction("Jump");
        dash = InputManager.Instance.GetGameplayAction("Dash");

        jump.performed += _ => JumpedThisFrame = true;
        dash.performed += _ => DashedThisFrame = true;

        if (MobileDetector.IsMobile())
        {
            touchJumpButton.OnPressed += () => JumpedThisFrame = true;
            touchDashButton.OnPressed += () => DashedThisFrame = true;
        }
    }

    private void Update()
    {
        Movement = MobileDetector.IsMobile()
            ? touchJoystick.Direction
            : move?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public void ResetJumpFlag()
    {
        JumpedThisFrame = false;
    }

    public void ResetDashFlag()
    {
        DashedThisFrame = false;
    }

    // Touch jump has no held state, so early-fall shortening only applies to keyboard/gamepad jumps
    public bool IsJumpHeld() => !MobileDetector.IsMobile() && (jump?.IsPressed() ?? false);
}