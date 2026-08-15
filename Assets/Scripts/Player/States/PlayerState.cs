using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController controller;
    protected PlayerStateMachine stateMachine;

    public PlayerState(PlayerController controller, PlayerStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

    protected PlayerState Run => stateMachine.RunState;
    protected PlayerState Idle => stateMachine.IdleState;
    protected PlayerState Jump => stateMachine.JumpState;
    protected PlayerState Fall => stateMachine.FallState;
    protected PlayerState Dash => stateMachine.DashState;
}