using System.Collections.Generic;
using BFTools.Core.EventBus;
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

    // Not spawned yet - a chunk's hazard markers land here at chunk-spawn time and
    // only turn into a pooled instance once the build line reaches their column,
    // via HandleColumnBuilding. targetList is the owning chunk's hazardInstances
    // list (a reference, so appending to it after the chunk's already been
    // enqueued still reaches the same ActiveChunk for release on unload).
    private struct PendingHazard
    {
        public int column;
        public string poolKey;
        public Vector3 spawnPosition;
        public List<GameObject> targetList;
    }

    // A hazard that's already spawned and pooled, tracked by column so a decayed
    // column can find and disable its collider - mirrors TileDecayer disabling a
    // tile's collider ahead of its visual removal. renderer/collider are cached
    // at spawn time so the build/decay steps don't need to GetComponent again.
    private struct ActiveHazard
    {
        public int column;
        public GameObject instance;
        public SpriteRenderer renderer;
        public Collider2D collider;
    }

    [SerializeField] private List<GameObject> chunkPrefabs;
    [SerializeField] private Tilemap masterTilemap;
    [SerializeField] private Transform spawnBoundary;
    [SerializeField] private Transform unloadBoundary;
    [SerializeField] private TileBuilder tileBuilder;
    [SerializeField] private TileDecayer tileDecayer;

    [Tooltip("Uniform chunk width in cells/units")]
    [SerializeField] private int chunkWidth = 20;

    private BFObjectPooler pooler;
    private readonly Queue<ActiveChunk> activeChunks = new Queue<ActiveChunk>();
    private readonly List<PendingHazard> pendingHazards = new List<PendingHazard>();
    private readonly List<ActiveHazard> activeHazards = new List<ActiveHazard>();
    private readonly HashSet<int> decayedColumnsScratch = new HashSet<int>();
    private int nextSpawnCellX;
    private bool isActive = true;

    private void Awake()
    {
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

    private void OnEnable()
    {
        tileBuilder.OnColumnBuilding += HandleColumnBuilding;
        tileDecayer.OnColumnDecayed += HandleColumnDecayed;
    }

    private void OnDisable()
    {
        tileBuilder.OnColumnBuilding -= HandleColumnBuilding;
        tileDecayer.OnColumnDecayed -= HandleColumnDecayed;
    }

    private void Start()
    {
        pooler = BFServiceLocator.Get<BFObjectPooler>();
        nextSpawnCellX = masterTilemap.WorldToCell(transform.position).x;
    }

    private void Update()
    {
        if (!isActive) return;

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

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState != GameState.Dying) return;

        isActive = false;
    }

    private void SpawnChunk()
    {
        GameObject chunkPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        Tilemap chunkTilemap = chunkPrefab.GetComponentInChildren<Tilemap>();

        BoundsInt sourceBounds = chunkTilemap.cellBounds;
        TileBase[] tiles = chunkTilemap.GetTilesBlock(sourceBounds);

        BoundsInt destBounds = new BoundsInt(nextSpawnCellX, sourceBounds.yMin, 0, sourceBounds.size.x, sourceBounds.size.y, 1);
        masterTilemap.SetTilesBlock(destBounds, tiles);
        HidePendingTiles(destBounds);

        List<GameObject> hazardInstances = new List<GameObject>();
        foreach (ChunkHazardMarker marker in chunkPrefab.GetComponentsInChildren<ChunkHazardMarker>(true))
        {
            Vector3 localOffset = chunkPrefab.transform.InverseTransformPoint(marker.transform.position);
            Vector3 spawnPosition = new Vector3(nextSpawnCellX + localOffset.x, localOffset.y, 0f);
            int column = masterTilemap.WorldToCell(spawnPosition).x;

            pendingHazards.Add(new PendingHazard
            {
                column = column,
                poolKey = marker.PoolKey,
                spawnPosition = spawnPosition,
                targetList = hazardInstances
            });
        }

        activeChunks.Enqueue(new ActiveChunk { tileBounds = destBounds, hazardInstances = hazardInstances });
        nextSpawnCellX += chunkWidth;
    }

    // Mirrors the build line reaching a column of tiles: any hazard marker queued
    // for that same column spawns now instead of at chunk-spawn time, so a spike
    // never appears (or becomes lethal) ahead of the world actually assembling
    // around it.
    private void HandleColumnBuilding(IReadOnlyList<Vector3Int> cells)
    {
        if (cells.Count == 0) return;

        int column = cells[0].x;

        for (int i = pendingHazards.Count - 1; i >= 0; i--)
        {
            PendingHazard pending = pendingHazards[i];
            if (pending.column != column) continue;

            GameObject instance = pooler.Get(pending.poolKey);
            instance.transform.position = pending.spawnPosition;
            pending.targetList.Add(instance);

            instance.TryGetComponent(out SpriteRenderer renderer);
            instance.TryGetComponent(out Collider2D collider);
            activeHazards.Add(new ActiveHazard { column = column, instance = instance, renderer = renderer, collider = collider });

            BeginHazardBuild(pending.spawnPosition, renderer, collider);

            pendingHazards.RemoveAt(i);
        }
    }

    // Hides the real hazard and plays the same assembly shader tiles use at this
    // position, revealing (and re-enabling its collider) only once that fragment
    // finishes - identical timing rule to BuildFragmentSpawner.RevealTile.
    private void BeginHazardBuild(Vector3 worldPosition, SpriteRenderer renderer, Collider2D collider)
    {
        if (renderer == null || renderer.sprite == null)
        {
            if (collider != null) collider.enabled = true;
            return;
        }

        renderer.enabled = false;
        if (collider != null) collider.enabled = false;

        GameObject fragment = pooler.Get(BuildFragmentController.PoolKey);
        if (fragment == null)
        {
            renderer.enabled = true;
            if (collider != null) collider.enabled = true;
            return;
        }

        fragment.GetComponent<BuildFragmentController>().Begin(renderer.sprite, worldPosition, () =>
        {
            renderer.enabled = true;
            if (collider != null) collider.enabled = true;
        });
    }

    // Mirrors TileDecayer disabling a tile's collider ahead of its visual removal -
    // a hazard sitting in a decayed column shouldn't stay lethal/solid after the
    // ground under it is already gone. DecayAll (on player death) reports every
    // decayed column in one call, not just the latest one, hence the column set.
    // The hazard's sprite is hidden the same way a decayed tile's is, with the
    // same crumble shader played in its place.
    private void HandleColumnDecayed(IReadOnlyList<Vector3Int> cells)
    {
        if (cells.Count == 0) return;

        decayedColumnsScratch.Clear();
        for (int i = 0; i < cells.Count; i++)
        {
            decayedColumnsScratch.Add(cells[i].x);
        }

        for (int i = 0; i < activeHazards.Count; i++)
        {
            ActiveHazard hazard = activeHazards[i];
            if (!decayedColumnsScratch.Contains(hazard.column)) continue;

            if (hazard.collider != null) hazard.collider.enabled = false;

            if (hazard.renderer == null || !hazard.renderer.enabled) continue;

            Vector3 worldPosition = hazard.instance.transform.position;
            Sprite sprite = hazard.renderer.sprite;
            hazard.renderer.enabled = false;

            if (sprite == null) continue;

            GameObject fragment = pooler.Get(DecayFragmentController.PoolKey);
            if (fragment == null) continue;

            fragment.GetComponent<DecayFragmentController>().Begin(sprite, worldPosition);
        }
    }

    // Newly placed tiles are real and connected to their neighbors right away
    // (so rule-tile matching resolves correctly against the previous chunk), but
    // stay invisible and non-solid until TileBuilder's build line reaches them.
    private void HidePendingTiles(BoundsInt bounds)
    {
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!masterTilemap.HasTile(cell)) continue;

            masterTilemap.SetTileFlags(cell, TileFlags.None);
            masterTilemap.SetColor(cell, Color.clear);
            masterTilemap.SetColliderType(cell, Tile.ColliderType.None);
        }
    }

    private void UnloadChunk(ActiveChunk chunk)
    {
        TileBase[] emptyTiles = new TileBase[chunk.tileBounds.size.x * chunk.tileBounds.size.y];
        masterTilemap.SetTilesBlock(chunk.tileBounds, emptyTiles);

        foreach (GameObject hazard in chunk.hazardInstances)
        {
            pooler.Release(hazard);

            for (int i = activeHazards.Count - 1; i >= 0; i--)
            {
                if (activeHazards[i].instance != hazard) continue;

                activeHazards.RemoveAt(i);
                break;
            }
        }
    }
}