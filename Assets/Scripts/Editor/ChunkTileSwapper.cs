using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Bulk-swaps one tile asset for another across every chunk prefab's tilemap(s).
// Tiles are referenced by asset reference internally, not by name, so this is a
// safe structural swap rather than a text find/replace - Unity's own
// Tilemap.SwapTile handles it per-tilemap once given both tile assets.
public static class ChunkTileSwapper
{
    // Edit these two names to swap a different pair later - matched exactly
    // against the tile asset's file name (without extension).
    private const string SourceTileName = "TemplateRuleTile";
    private const string TargetTileName = "New Rule Tile";

    [MenuItem("Tools/Chunks/Swap Tile In All Chunk Prefabs")]
    private static void SwapTileInAllChunkPrefabs()
    {
        TileBase sourceTile = FindTileAsset(SourceTileName);
        TileBase targetTile = FindTileAsset(TargetTileName);

        if (sourceTile == null)
        {
            Debug.LogError($"[ChunkTileSwapper] Could not find a tile asset named \"{SourceTileName}\".");
            return;
        }

        if (targetTile == null)
        {
            Debug.LogError($"[ChunkTileSwapper] Could not find a tile asset named \"{TargetTileName}\".");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int prefabsChanged = 0;
        int tilemapsChanged = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            bool prefabChanged = false;

            foreach (Tilemap tilemap in prefab.GetComponentsInChildren<Tilemap>(true))
            {
                if (!TilemapUsesTile(tilemap, sourceTile)) continue;

                tilemap.SwapTile(sourceTile, targetTile);
                EditorUtility.SetDirty(tilemap);
                tilemapsChanged++;
                prefabChanged = true;
            }

            if (prefabChanged)
            {
                EditorUtility.SetDirty(prefab);
                prefabsChanged++;
            }
        }

        AssetDatabase.SaveAssets();

        Debug.Log($"[ChunkTileSwapper] Swapped \"{SourceTileName}\" -> \"{TargetTileName}\" in {tilemapsChanged} tilemap(s) across {prefabsChanged} prefab(s).");
    }

    private static bool TilemapUsesTile(Tilemap tilemap, TileBase tile)
    {
        foreach (Vector3Int cell in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.GetTile(cell) == tile) return true;
        }

        return false;
    }

    private static TileBase FindTileAsset(string tileName)
    {
        foreach (string guid in AssetDatabase.FindAssets($"t:TileBase {tileName}"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == tileName)
            {
                return AssetDatabase.LoadAssetAtPath<TileBase>(path);
            }
        }

        return null;
    }
}