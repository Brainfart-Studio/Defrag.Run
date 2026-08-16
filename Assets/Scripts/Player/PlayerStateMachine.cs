using BFTools.Core.EventBus;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController controller;

    private PlayerState currentState;
    private PlayerState fallState;
    private PlayerState idleState;
    private PlayerState runState;
    private PlayerState jumpState;
    private PlayerState dashState;

    private bool isFrozen;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        fallState = new FallState(controller, this);
        idleState = new IdleState(controller, this);
        runState = new RunState(controller, this);
        jumpState = new JumpState(controller, this);
        dashState = new DashState(controller, this);

        ChangeState(fallState);

        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void Update()
    {
        if (isFrozen) return;
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        if (isFrozen) return;
        currentState?.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.Dying) return;

        isFrozen = true;
        controller.GetRigidbody().velocity = Vector2.zero;
    }

    // Expose preallocated states for transitions
    public PlayerState FallState => fallState;
    public PlayerState IdleState => idleState;
    public PlayerState RunState => runState;
    public PlayerState JumpState => jumpState;
    public PlayerState DashState => dashState;

}