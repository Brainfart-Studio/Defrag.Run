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

    // TEMP: replace with OnGameStart hookup once the start line trigger exists
    private void Start()
    {
        BeginScrolling();
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