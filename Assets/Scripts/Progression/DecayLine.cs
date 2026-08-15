using UnityEngine;

public class DecayLine : MonoBehaviour
{
    [Tooltip("Decay line scroll speed with zero difficulty")]
    [SerializeField] private float baseSpeed = 3f;

    [Tooltip("Additional scroll speed per point of difficulty. Keep slightly higher than the camera's multiplier so the safe zone shrinks over time")]
    [SerializeField] private float difficultySpeedMultiplier = 1.01f;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position + Vector3.up * 10f, transform.position + Vector3.down * 10f);
    }
}