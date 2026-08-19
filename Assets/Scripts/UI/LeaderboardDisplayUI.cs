using BFTools.Core.EventBus;
using Dan.Main;
using TMPro;
using UnityEngine;

public class LeaderboardDisplayUI : MonoBehaviour
{
    [SerializeField] private LeaderboardEntryView entryPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.GameOver) return;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        statusText.text = "Loading leaderboard...";
        statusText.gameObject.SetActive(true);

        LeaderboardCreator.GetLeaderboard(LeaderboardKeys.WannaJam2026, Populate, OnLeaderboardError);
    }

    private void Populate(Dan.Models.Entry[] entries)
    {
        statusText.gameObject.SetActive(false);

        foreach (Dan.Models.Entry entry in entries)
        {
            LeaderboardEntryView view = Instantiate(entryPrefab, contentParent);
            view.SetEntry(entry);
        }
    }

    private void OnLeaderboardError(string errorMessage)
    {
        statusText.text = "Leaderboard unavailable offline.";
        statusText.gameObject.SetActive(true);
    }
}