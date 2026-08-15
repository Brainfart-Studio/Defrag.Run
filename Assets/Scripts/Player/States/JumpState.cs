using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController controller, PlayerStateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Enter()
    {
        controller.Jump();
        controller.ConsumeJump();
    }

    public override void Update()
    {
        if (controller.IsDashInputValid())
        {
            stateMachine.ChangeState(Dash);
        }
        else if (controller.GetRigidbody().velocity.y <= 0)
        {
            stateMachine.ChangeState(Fall);
        }
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.GetInputHandler().Movement.x);
        controller.ApplyEarlyFallMultiplier();
    }
}