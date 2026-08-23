using BFTools.Core.EventBus;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [Tooltip("Camera scroll speed with zero difficulty")]
    [SerializeField] private float baseSpeed = 3f;

    [Tooltip("Additional scroll speed per point of difficulty")]
    [SerializeField] private float difficultySpeedMultiplier = 1f;

    [Tooltip("Speed cap. Once reached, stops scaling with difficulty and holds this speed")]
    [SerializeField] private float maxSpeed = 10f;

    private bool isActive;
    private bool isScaling = true;
    private float currentSpeed;

    public float CurrentSpeed => currentSpeed;

    private void Awake()
    {
        EventBus<GameStartEvent>.Subscribe(OnGameStart);
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStartEvent>.Unsubscribe(OnGameStart);
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void Update()
    {
        if (!isActive) return;

        if (isScaling)
        {
            currentSpeed = CalculateScrollSpeed(DifficultyManager.Instance.Difficulty);
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
                isScaling = false;
            }
        }

        transform.position += Vector3.right * currentSpeed * Time.deltaTime;
    }

    private void OnGameStart(GameStartEvent e)
    {
        BeginScrolling();
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.Dying) return;

        isActive = false;
    }

    private float CalculateScrollSpeed(float difficulty)
    {
        return baseSpeed + difficulty * difficultySpeedMultiplier;
    }

    public void BeginScrolling()
    {
        isActive = true;
    }
}