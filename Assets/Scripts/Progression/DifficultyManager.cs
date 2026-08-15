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
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
    }

    // TEMP: replace with OnGameStart hookup once the start line trigger exists
    private void Start()
    {
        BeginScaling();
    }

    private void Update()
    {
        if (!isActive) return;

        Difficulty = CalculateDifficulty(ScoreManager.Instance.MaxDistance);
        Debug.Log(Difficulty);
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