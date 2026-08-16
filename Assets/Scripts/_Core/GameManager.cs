using System.Collections;
using BFTools.Core.EventBus;
using UnityEngine;

public enum GameState
{
    Playing,
    Dying,
    HighScore,
    GameOver
}

public struct GameStateChangedEvent
{
    public GameState NewState;

    public GameStateChangedEvent(GameState newState)
    {
        NewState = newState;
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Tooltip("Time to hold in the Dying state before moving to GameOver. Placeholder until death visuals (particles, tile break-fall) are wired up")]
    [SerializeField] private float deathSequenceDuration = 1.5f;

    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        instance = this;
        InputManager.Instance.EnableGameplay();
        EventBus<PlayerDeathEvent>.Subscribe(OnPlayerDeath);
    }

    private void OnDestroy()
    {
        EventBus<PlayerDeathEvent>.Unsubscribe(OnPlayerDeath);
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        ChangeState(GameState.Dying);
        StartCoroutine(EndDeathSequence());
    }

    private IEnumerator EndDeathSequence()
    {
        yield return new WaitForSeconds(deathSequenceDuration);
        ChangeState(GameState.GameOver);
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        EventBus<GameStateChangedEvent>.Fire(new GameStateChangedEvent(newState));
    }
}