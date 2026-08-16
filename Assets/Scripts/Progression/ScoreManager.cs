using BFTools.Core.EventBus;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance => instance;

    private Transform player;
    private bool isTracking;

    public float MaxDistance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        EventBus<GameStartEvent>.Subscribe(OnGameStart);
    }

    private void Update()
    {
        if (!isTracking || player == null) return;

        if (player.position.x > MaxDistance)
        {
            MaxDistance = Mathf.Round(player.position.x);
        }
    }

    private void OnGameStart(GameStartEvent e)
    {
        BeginTracking();
    }

    public void BeginTracking()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
        MaxDistance = player != null ? Mathf.Round(player.position.x) : 0f;
        isTracking = true;
    }
}