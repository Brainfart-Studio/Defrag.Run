using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Editor-only: adds a button to ChunkLoader's inspector that scans
// Assets/Prefabs/Chunks and fills Chunk Prefabs with every prefab found
// there (excluding the authoring template), sorted by name so re-running
// it produces a stable, diffable order.
[CustomEditor(typeof(ChunkLoader))]
public class ChunkLoaderEditor : Editor
{
    private const string ChunksFolder = "Assets/Prefabs/Chunks";
    private const string ExcludedName = "_Template";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Populate Chunk Prefabs From Folder"))
        {
            PopulateChunkPrefabs();
        }
    }

    private void PopulateChunkPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(ChunksFolder))
        {
            Debug.LogError($"ChunkLoaderEditor: folder not found: {ChunksFolder}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { ChunksFolder });

        GameObject[] prefabs = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => Path.GetFileNameWithoutExtension(path) != ExcludedName)
            .OrderBy(Path.GetFileNameWithoutExtension)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .Where(prefab => prefab != null)
            .ToArray();

        SerializedProperty chunkPrefabsProperty = serializedObject.FindProperty("chunkPrefabs");
        chunkPrefabsProperty.arraySize = prefabs.Length;

        for (int i = 0; i < prefabs.Length; i++)
        {
            chunkPrefabsProperty.GetArrayElementAtIndex(i).objectReferenceValue = prefabs[i];
        }

        serializedObject.ApplyModifiedProperties();

        Debug.Log($"ChunkLoaderEditor: populated {prefabs.Length} chunk prefab(s) from {ChunksFolder}");
    }
}