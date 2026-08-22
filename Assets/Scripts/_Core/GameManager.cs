using System.Collections;
using BFTools.Core.EventBus;
using BFTools.Feedback.ScreenFlash;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    Playing,
    Paused,
    Dying,
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

    [Tooltip("Time to hold in the Dying state before moving to HighScore. Placeholder until death visuals (particles, tile break-fall) are wired up")]
    [SerializeField] private float deathSequenceDuration = 1.5f;

    public GameState CurrentState { get; private set; } = GameState.Playing;

    private InputAction gameplayPauseAction;
    private InputAction menuPauseAction;

    private void Awake()
    {
        instance = this;
        InputManager.Instance.EnableGameplay();
        EventBus<PlayerDeathEvent>.Subscribe(OnPlayerDeath);

        gameplayPauseAction = InputManager.Instance.GetGameplayAction("Pause");
        menuPauseAction = InputManager.Instance.GetMenuAction("Pause");
        gameplayPauseAction.performed += OnPausePerformed;
        menuPauseAction.performed += OnPausePerformed;
    }

    private void OnDestroy()
    {
        EventBus<PlayerDeathEvent>.Unsubscribe(OnPlayerDeath);
        gameplayPauseAction.performed -= OnPausePerformed;
        menuPauseAction.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        if (CurrentState == GameState.Playing)
        {
            Pause();
        }
        else if (CurrentState == GameState.Paused)
        {
            Resume();
        }
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing) return;

        Time.timeScale = 0f;
        InputManager.Instance.EnableMenu();
        ChangeState(GameState.Paused);
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused) return;

        Time.timeScale = 1f;
        InputManager.Instance.EnableGameplay();
        ChangeState(GameState.Playing);
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        if (CurrentState != GameState.Playing) return;

        EventBus<BFScreenFlashEvent>.Fire(new BFScreenFlashEvent { eventName = "Death" });
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