using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBuilder : MonoBehaviour
{
    [SerializeField] private Transform buildLine;
    [SerializeField] private Tilemap masterTilemap;

    [Tooltip("Vertical cell range to check per column, centered on row 0")]
    [SerializeField] private int rowRangeMin = -15;
    [SerializeField] private int rowRangeMax = 15;

    private bool isActive;
    private int lastProcessedColumn;
    private readonly List<Vector3Int> buildingCells = new List<Vector3Int>();

    public event Action<IReadOnlyList<Vector3Int>> OnColumnBuilding;

    // TEMP: replace with OnGameStart hookup once the start line trigger exists
    private void Start()
    {
        lastProcessedColumn = masterTilemap.WorldToCell(buildLine.position).x;
        BeginBuilding();
    }

    private void Update()
    {
        if (!isActive) return;

        int currentColumn = masterTilemap.WorldToCell(buildLine.position).x;

        while (lastProcessedColumn < currentColumn)
        {
            lastProcessedColumn++;
            BuildColumn(lastProcessedColumn);
        }
    }

    private void BuildColumn(int column)
    {
        buildingCells.Clear();

        // Only identifies which cells are ready to start assembling. Collider
        // and visibility stay owned by BuildFragmentSpawner, deferred until each
        // cell's assembly fragment actually finishes - not fired from here.
        for (int row = rowRangeMin; row <= rowRangeMax; row++)
        {
            Vector3Int cell = new Vector3Int(column, row, 0);
            if (!masterTilemap.HasTile(cell)) continue;

            buildingCells.Add(cell);
        }

        if (buildingCells.Count > 0)
        {
            OnColumnBuilding?.Invoke(buildingCells);
        }
    }

    public void BeginBuilding()
    {
        isActive = true;
    }
}