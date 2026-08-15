using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Update()
    {
        if (controller.IsDashInputValid())
        {
            stateMachine.ChangeState(Dash);
        }
        else if (Mathf.Abs(controller.GetInputHandler().Movement.x) < 0.1f)
        {
            stateMachine.ChangeState(Idle);
        }
        else if (controller.IsJumpInputValid())
        {
            stateMachine.ChangeState(Jump);
        }
        else if (!controller.IsGrounded)
        {
            stateMachine.ChangeState(Fall);
        }
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.GetInputHandler().Movement.x);
    }
}