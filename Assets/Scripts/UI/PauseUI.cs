using BFTools.Core.EventBus;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;

    private void Awake()
    {
        panelRoot.SetActive(false);
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.Paused && e.NewState != GameState.Playing) return;

        panelRoot.SetActive(e.NewState == GameState.Paused);
    }
}