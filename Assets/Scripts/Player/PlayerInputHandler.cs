using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
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
    }

    private void Update()
    {
        Movement = move?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public void ResetJumpFlag()
    {
        JumpedThisFrame = false;
    }

    public void ResetDashFlag()
    {
        DashedThisFrame = false;
    }

    public bool IsJumpHeld() => jump?.IsPressed() ?? false;
}