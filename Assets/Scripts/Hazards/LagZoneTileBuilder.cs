using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LagZoneTileBuilder : MonoBehaviour
{
    [SerializeField] private Tilemap lagZoneTilemap;
    [SerializeField] private TileBuilder tileBuilder;
    [SerializeField] private TileDecayer tileDecayer;

    [Tooltip("Collider type to restore once a column finishes building. Must match the lag zone tile assets' configured collider type")]
    [SerializeField] private Tile.ColliderType restoredColliderType = Tile.ColliderType.Sprite;

    [Tooltip("Vertical cell range to check per column, centered on row 0")]
    [SerializeField] private int rowRangeMin = -15;
    [SerializeField] private int rowRangeMax = 15;

    private readonly HashSet<int> decayedColumnsScratch = new HashSet<int>();

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

    // Rides the same build line as the ground tilemap, but reads its own column
    // independently rather than reusing the reported cells - those cells are
    // ground tile positions, and a lag zone can sit at rows with no ground tile
    // under it at all.
    private void HandleColumnBuilding(IReadOnlyList<Vector3Int> cells)
    {
        if (cells.Count == 0) return;

        RevealColumn(cells[0].x);
    }

    private void RevealColumn(int column)
    {
        for (int row = rowRangeMin; row <= rowRangeMax; row++)
        {
            Vector3Int cell = new Vector3Int(column, row, 0);
            if (!lagZoneTilemap.HasTile(cell)) continue;

            lagZoneTilemap.SetTileFlags(cell, TileFlags.None);
            lagZoneTilemap.SetColor(cell, Color.white);
            lagZoneTilemap.SetColliderType(cell, restoredColliderType);
        }
    }

    // DecayAll (on player death) reports every decayed column in one call, not
    // just the latest one, so every distinct column in the batch needs decaying -
    // same reasoning as ChunkLoader.HandleColumnDecayed.
    private void HandleColumnDecayed(IReadOnlyList<Vector3Int> cells)
    {
        if (cells.Count == 0) return;

        decayedColumnsScratch.Clear();
        for (int i = 0; i < cells.Count; i++)
        {
            decayedColumnsScratch.Add(cells[i].x);
        }

        foreach (int column in decayedColumnsScratch)
        {
            DecayColumn(column);
        }
    }

    private void DecayColumn(int column)
    {
        for (int row = rowRangeMin; row <= rowRangeMax; row++)
        {
            Vector3Int cell = new Vector3Int(column, row, 0);
            if (!lagZoneTilemap.HasTile(cell)) continue;

            lagZoneTilemap.SetColliderType(cell, Tile.ColliderType.None);
        }
    }
}