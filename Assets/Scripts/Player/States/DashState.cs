using UnityEngine;

public class DashState : PlayerState
{
    public DashState(PlayerController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Enter()
    {
        controller.Dash();
        controller.ConsumeDash();
    }

    public override void Update()
    {
        if (controller.IsDashing) return;

        if (controller.IsGrounded)
        {
            var move = controller.GetInputHandler().Movement.magnitude;
            stateMachine.ChangeState(move < 0.1f ? Idle : Run);
        }
        else
        {
            stateMachine.ChangeState(Fall);
        }
    }
}