using BFTools.Core.EventBus;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private static DifficultyManager instance;
    public static DifficultyManager Instance => instance;

    [Tooltip("Distance divisor for the difficulty formula. Higher = slower ramp")]
    [SerializeField] private float distanceDivisor = 100f;

    private bool isActive;

    public float Difficulty { get; private set; }

    private void Awake()
    {
        instance = this;
        EventBus<GameStartEvent>.Subscribe(OnGameStart);
    }

    private void Update()
    {
        if (!isActive) return;

        Difficulty = CalculateDifficulty(ScoreManager.Instance.MaxDistance);
    }

    private void OnGameStart(GameStartEvent e)
    {
        BeginScaling();
    }

    private float CalculateDifficulty(float distance)
    {
        return distance / distanceDivisor;
    }

    public void BeginScaling()
    {
        isActive = true;
    }
}