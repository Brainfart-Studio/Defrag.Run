using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

// Playtest-only aid: tints each spawned chunk a random color and labels it with
// the chunk prefab's name, so a playtester can tell exactly which chunk
// an issue happened in. Purely a listener on ChunkLoader's spawn/unload events -
// ChunkLoader has no idea this exists. Safe to disable or delete outright before
// a release build; see enableDebugOverlay below.
public class ChunkDebugOverlay : MonoBehaviour
{
    private struct DebugVisual
    {
        public GameObject tint;
        public GameObject label;
    }

    [Tooltip("Master on/off switch for this overlay. Flip off (or delete this GameObject) before a release build")]
    [SerializeField] private bool enableDebugOverlay = true;

    [SerializeField] private ChunkLoader chunkLoader;
    [SerializeField] private Tilemap masterTilemap;

    [Tooltip("Alpha of the random chunk tint - low enough that tiles/hazards stay readable through it")]
    [SerializeField, Range(0f, 1f)] private float tintAlpha = 0.25f;

    [Tooltip("Sorting order for the tint quad - keep well below the tilemap/hazard sprites so it never occludes gameplay")]
    [SerializeField] private int tintSortingOrder = -100;

    private static Sprite whitePixelSprite;

    // Chunks unload strictly in the order they spawned (same FIFO ChunkLoader
    // uses for activeChunks), so a plain queue stays in sync without needing to
    // match visuals back to a chunk by bounds.
    private readonly Queue<DebugVisual> activeVisuals = new Queue<DebugVisual>();

    private void Awake()
    {
        if (!enableDebugOverlay || !Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        chunkLoader.OnChunkSpawned += HandleChunkSpawned;
        chunkLoader.OnChunkUnloaded += HandleChunkUnloaded;
    }

    private void OnDisable()
    {
        chunkLoader.OnChunkSpawned -= HandleChunkSpawned;
        chunkLoader.OnChunkUnloaded -= HandleChunkUnloaded;
    }

    private void HandleChunkSpawned(GameObject chunkPrefab, BoundsInt bounds)
    {
        Vector3 worldMin = masterTilemap.CellToWorld(bounds.min);
        Vector3 worldMax = masterTilemap.CellToWorld(bounds.max);
        Vector3 center = (worldMin + worldMax) / 2f;
        Vector3 size = worldMax - worldMin;

        GameObject tint = new GameObject($"ChunkDebugTint ({chunkPrefab.name})");
        tint.transform.SetParent(transform, false);
        tint.transform.position = center;
        tint.transform.localScale = size;

        SpriteRenderer renderer = tint.AddComponent<SpriteRenderer>();
        renderer.sprite = GetWhitePixelSprite();
        renderer.sortingOrder = tintSortingOrder;
        Color color = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.8f, 1f);
        color.a = tintAlpha;
        renderer.color = color;

        GameObject label = new GameObject($"ChunkDebugLabel ({chunkPrefab.name})");
        label.transform.SetParent(transform, false);
        label.transform.position = new Vector3(worldMin.x, worldMax.y, 0f);

        TextMeshPro text = label.AddComponent<TextMeshPro>();
        text.text = chunkPrefab.name;
        text.fontSize = 3f;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;

        activeVisuals.Enqueue(new DebugVisual { tint = tint, label = label });
    }

    private void HandleChunkUnloaded(BoundsInt bounds)
    {
        if (activeVisuals.Count == 0) return;

        DebugVisual visual = activeVisuals.Dequeue();
        Destroy(visual.tint);
        Destroy(visual.label);
    }

    private static Sprite GetWhitePixelSprite()
    {
        if (whitePixelSprite != null) return whitePixelSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whitePixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return whitePixelSprite;
    }
}