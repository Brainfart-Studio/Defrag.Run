using UnityEngine;

public class ChunkMetadata : MonoBehaviour
{
    [Tooltip("Where this chunk sits on the difficulty curve. Weighed against DifficultyManager.Difficulty when the spawner picks a chunk")]
    [field: SerializeField]
    public float DifficultyRating { get; private set; }
}