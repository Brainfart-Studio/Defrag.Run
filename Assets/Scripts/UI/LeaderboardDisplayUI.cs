using BFTools.Core.EventBus;
using Dan.Main;
using UnityEngine;

public class LeaderboardDisplayUI : MonoBehaviour
{
    [SerializeField] private LeaderboardEntryView entryPrefab;
    [SerializeField] private Transform contentParent;

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

        LeaderboardCreator.GetLeaderboard(LeaderboardKeys.WannaJam2026, Populate);
    }

    private void Populate(Dan.Models.Entry[] entries)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Dan.Models.Entry entry in entries)
        {
            LeaderboardEntryView view = Instantiate(entryPrefab, contentParent);
            view.SetEntry(entry);
        }
    }
}