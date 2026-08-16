using System.Collections.Generic;
using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkLoader : MonoBehaviour
{
    private struct ActiveChunk
    {
        public BoundsInt tileBounds;
        public List<GameObject> hazardInstances;
    }

    [SerializeField] private List<GameObject> chunkPrefabs;
    [SerializeField] private Tilemap masterTilemap;
    [SerializeField] private Transform spawnBoundary;
    [SerializeField] private Transform unloadBoundary;

    [Tooltip("Uniform chunk width in cells/units")]
    [SerializeField] private int chunkWidth = 20;

    private BFObjectPooler pooler;
    private readonly Queue<ActiveChunk> activeChunks = new Queue<ActiveChunk>();
    private int nextSpawnCellX;

    private void Start()
    {
        pooler = BFServiceLocator.Get<BFObjectPooler>();
        nextSpawnCellX = masterTilemap.WorldToCell(transform.position).x;
    }

    private void Update()
    {
        while (nextSpawnCellX < masterTilemap.WorldToCell(spawnBoundary.position).x)
        {
            SpawnChunk();
        }

        int unloadCellX = masterTilemap.WorldToCell(unloadBoundary.position).x;
        while (activeChunks.Count > 0 && activeChunks.Peek().tileBounds.xMax <= unloadCellX)
        {
            UnloadChunk(activeChunks.Dequeue());
        }
    }

    private void SpawnChunk()
    {
        GameObject chunkPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        Tilemap chunkTilemap = chunkPrefab.GetComponentInChildren<Tilemap>();

        BoundsInt sourceBounds = chunkTilemap.cellBounds;
        TileBase[] tiles = chunkTilemap.GetTilesBlock(sourceBounds);

        BoundsInt destBounds = new BoundsInt(nextSpawnCellX, sourceBounds.yMin, 0, sourceBounds.size.x, sourceBounds.size.y, 1);
        masterTilemap.SetTilesBlock(destBounds, tiles);

        List<GameObject> hazardInstances = new List<GameObject>();
        foreach (ChunkHazardMarker marker in chunkPrefab.GetComponentsInChildren<ChunkHazardMarker>(true))
        {
            Vector3 localOffset = chunkPrefab.transform.InverseTransformPoint(marker.transform.position);
            Vector3 spawnPosition = new Vector3(nextSpawnCellX + localOffset.x, localOffset.y, 0f);

            GameObject instance = pooler.Get(marker.PoolKey);
            instance.transform.position = spawnPosition;
            hazardInstances.Add(instance);
        }

        activeChunks.Enqueue(new ActiveChunk { tileBounds = destBounds, hazardInstances = hazardInstances });
        nextSpawnCellX += chunkWidth;
    }

    private void UnloadChunk(ActiveChunk chunk)
    {
        TileBase[] emptyTiles = new TileBase[chunk.tileBounds.size.x * chunk.tileBounds.size.y];
        masterTilemap.SetTilesBlock(chunk.tileBounds, emptyTiles);

        foreach (GameObject hazard in chunk.hazardInstances)
        {
            pooler.Release(hazard);
        }
    }
}