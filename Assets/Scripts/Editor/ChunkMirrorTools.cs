using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Editor-only tool: duplicates a chunk prefab and mirrors its contents
// left-right, saving the copy as "<Name>R" (e.g. Chunk1 -> Chunk1R).
// Tiles are reversed within the same bounds so the chunk's footprint and
// entry/exit edges don't move. Hazard markers are reflected about the main
// tilemap's center and have their local scale.x flipped as a starting point
// for visual facing - asymmetric hazards (lasers especially) still need a
// manual look/prefab swap afterward, this just gets the layout right.
public static class ChunkMirrorTool
{
    [MenuItem("Assets/Chunks/Create Mirrored Chunk", true)]
    private static bool ValidateCreateMirroredChunk()
    {
        return Selection.activeObject is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go);
    }

    [MenuItem("Assets/Chunks/Create Mirrored Chunk")]
    private static void CreateMirroredChunk()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is not GameObject prefabAsset || !PrefabUtility.IsPartOfPrefabAsset(prefabAsset)) continue;

            string sourcePath = AssetDatabase.GetAssetPath(prefabAsset);
            string directory = Path.GetDirectoryName(sourcePath);
            string mirroredName = prefabAsset.name + "R";
            string mirroredPath = Path.Combine(directory, mirroredName + ".prefab").Replace("\\", "/");

            if (File.Exists(mirroredPath))
            {
                Debug.LogWarning($"ChunkMirrorTool: {mirroredPath} already exists, skipping {prefabAsset.name}");
                continue;
            }

            AssetDatabase.CopyAsset(sourcePath, mirroredPath);
            AssetDatabase.ImportAsset(mirroredPath);

            GameObject contents = PrefabUtility.LoadPrefabContents(mirroredPath);
            MirrorChunkContents(contents);
            PrefabUtility.SaveAsPrefabAsset(contents, mirroredPath);
            PrefabUtility.UnloadPrefabContents(contents);

            Debug.Log($"ChunkMirrorTool: Created {mirroredName} from {prefabAsset.name}");
        }

        AssetDatabase.Refresh();
    }

    private static void MirrorChunkContents(GameObject root)
    {
        Tilemap mainTilemap = root.GetComponentInChildren<Tilemap>();
        if (mainTilemap == null)
        {
            Debug.LogWarning($"ChunkMirrorTool: {root.name} has no Tilemap, nothing mirrored");
            return;
        }

        BoundsInt bounds = mainTilemap.cellBounds;
        float mirrorWorldX = (mainTilemap.CellToWorld(bounds.min).x + mainTilemap.CellToWorld(bounds.max).x) / 2f;

        foreach (Tilemap tilemap in root.GetComponentsInChildren<Tilemap>(true))
        {
            MirrorTilemap(tilemap);
        }

        foreach (ChunkHazardMarker marker in root.GetComponentsInChildren<ChunkHazardMarker>(true))
        {
            Vector3 pos = marker.transform.position;
            pos.x = 2f * mirrorWorldX - pos.x;
            marker.transform.position = pos;

            Vector3 scale = marker.transform.localScale;
            scale.x = -scale.x;
            marker.transform.localScale = scale;
        }
    }

    // Reverses each row of tiles left-to-right within the same bounds, so
    // the chunk's footprint and entry/exit edges stay exactly where they were.
    private static void MirrorTilemap(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] source = tilemap.GetTilesBlock(bounds);
        TileBase[] mirrored = new TileBase[source.Length];

        int width = bounds.size.x;
        int height = bounds.size.y;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int sourceIndex = y * width + x;
                int mirroredIndex = y * width + (width - 1 - x);
                mirrored[mirroredIndex] = source[sourceIndex];
            }
        }

        tilemap.SetTilesBlock(bounds, mirrored);
    }
}