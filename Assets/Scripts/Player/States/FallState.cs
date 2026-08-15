using UnityEngine;

public class FallState : PlayerState
{
    public FallState(PlayerController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Update()
    {
        if (controller.IsDashInputValid())
        {
            stateMachine.ChangeState(Dash);
        }
        else if (controller.IsJumpInputValid())
        {
            stateMachine.ChangeState(Jump);
        }
        else if (controller.IsGrounded)
        {
            var move = controller.GetInputHandler().Movement.magnitude;
            stateMachine.ChangeState(move < 0.1f ? Idle : Run);
        }
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.GetInputHandler().Movement.x);
        controller.LimitFallSpeed();
    }
}