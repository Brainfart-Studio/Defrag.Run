using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDecayer : MonoBehaviour
{
    [SerializeField] private Transform decayLine;
    [SerializeField] private Tilemap masterTilemap;

    [Tooltip("Vertical cell range to check per column, centered on row 0")]
    [SerializeField] private int rowRangeMin = -15;
    [SerializeField] private int rowRangeMax = 15;

    private bool isActive;
    private int lastProcessedColumn;

    public event Action<Vector3Int> OnTileDecayed;

    // TEMP: replace with OnGameStart hookup once the start line trigger exists
    private void Start()
    {
        lastProcessedColumn = masterTilemap.WorldToCell(decayLine.position).x;
        BeginDecay();
    }

    private void Update()
    {
        if (!isActive) return;

        int currentColumn = masterTilemap.WorldToCell(decayLine.position).x;

        while (lastProcessedColumn < currentColumn)
        {
            lastProcessedColumn++;
            DecayColumn(lastProcessedColumn);
        }
    }

    private void DecayColumn(int column)
    {
        for (int row = rowRangeMin; row <= rowRangeMax; row++)
        {
            Vector3Int cell = new Vector3Int(column, row, 0);
            if (!masterTilemap.HasTile(cell)) continue;

            masterTilemap.SetColliderType(cell, Tile.ColliderType.None);
            OnTileDecayed?.Invoke(cell);
        }
    }

    public void BeginDecay()
    {
        isActive = true;
    }
}