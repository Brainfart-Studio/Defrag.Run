using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Enter()
    {
        controller.StopHorizontalMovement();
    }

    public override void Update()
    {
        if (controller.IsDashInputValid())
        {
            stateMachine.ChangeState(Dash);
        }
        else if (Mathf.Abs(controller.GetInputHandler().Movement.x) > 0.1f)
        {
            stateMachine.ChangeState(Run);
        }
        else if (controller.IsJumpInputValid())
        {
            stateMachine.ChangeState(Jump);
        }
    }
}