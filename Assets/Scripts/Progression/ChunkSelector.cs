using System.Collections.Generic;
using UnityEngine;

public static class ChunkSelector
{
    // Chunks close to currentDifficulty are weighted more heavily, but minWeight
    // keeps every chunk pickable - with only a handful of chunks in the pool, a
    // hard cutoff risks leaving the spawner with nothing eligible.
    public static GameObject PickWeighted(IReadOnlyList<GameObject> chunkPrefabs, float currentDifficulty, float tolerance, float minWeight = 0.05f)
    {
        int count = chunkPrefabs.Count;
        float[] weights = new float[count];
        float totalWeight = 0f;

        for (int i = 0; i < count; i++)
        {
            float rating = chunkPrefabs[i].TryGetComponent(out ChunkMetadata metadata) ? metadata.DifficultyRating : 0f;
            float weight = Mathf.Max(minWeight, tolerance - Mathf.Abs(rating - currentDifficulty));
            weights[i] = weight;
            totalWeight += weight;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < count; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative) return chunkPrefabs[i];
        }

        return chunkPrefabs[count - 1];
    }
}