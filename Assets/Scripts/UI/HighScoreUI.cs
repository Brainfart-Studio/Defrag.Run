using BFTools.Core.EventBus;
using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        panelRoot.SetActive(false);
        submitButton.onClick.AddListener(OnSubmitPressed);
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        submitButton.onClick.RemoveListener(OnSubmitPressed);
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.HighScore)
        {
            panelRoot.SetActive(false);
            return;
        }

        nameInput.text = string.Empty;
        statusText.text = string.Empty;
        submitButton.interactable = true;
        InputManager.Instance.EnableMenu();
        panelRoot.SetActive(true);
    }

    private void OnSubmitPressed()
    {
        var playerName = string.IsNullOrWhiteSpace(nameInput.text) ? "Player" : nameInput.text.Trim();

        if (string.IsNullOrEmpty(LeaderboardCreator.UserGuid))
        {
            statusText.text = "Could not connect to leaderboard.";
            GameManager.Instance.CompleteHighScore();
            return;
        }

        submitButton.interactable = false;
        statusText.text = "Submitting...";

        LeaderboardCreator.UploadNewEntry(
            LeaderboardKeys.WannaJam2026,
            playerName,
            ScoreManager.Instance.Score,
            isSuccessful => GameManager.Instance.CompleteHighScore());
    }
}