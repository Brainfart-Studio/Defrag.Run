using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [Tooltip("Camera scroll speed with zero difficulty")]
    [SerializeField] private float baseSpeed = 3f;

    [Tooltip("Additional scroll speed per point of difficulty")]
    [SerializeField] private float difficultySpeedMultiplier = 1f;

    private bool isActive;

    private void Update()
    {
        if (!isActive) return;

        float speed = CalculateScrollSpeed(DifficultyManager.Instance.Difficulty);
        transform.position += Vector3.right * speed * Time.deltaTime;
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