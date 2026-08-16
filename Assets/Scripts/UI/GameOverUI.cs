using BFTools.Core.EventBus;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text scoreText;

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
        if (e.NewState != GameState.GameOver) return;

        scoreText.text = $"Score: {ScoreManager.Instance.MaxDistance}";
        InputManager.Instance.EnableMenu();
        panelRoot.SetActive(true);
    }
}