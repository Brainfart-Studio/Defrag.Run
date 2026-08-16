using System;
using System.Collections.Generic;
using BFTools.Core.EventBus;
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
    private readonly List<Vector3Int> decayedCells = new List<Vector3Int>();

    public event Action<IReadOnlyList<Vector3Int>> OnColumnDecayed;

    private void Awake()
    {
        EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
    }

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

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState == GameState.Dying)
        {
            DecayAll();
        }
    }

    private void DecayColumn(int column)
    {
        decayedCells.Clear();

        // Disable colliders and collect every decaying cell in this column first.
        // The sprite/removal pass runs after this loop so a cleared tile can't
        // trigger a rule-tile neighbor refresh on a cell we haven't read yet.
        for (int row = rowRangeMin; row <= rowRangeMax; row++)
        {
            Vector3Int cell = new Vector3Int(column, row, 0);
            if (!masterTilemap.HasTile(cell)) continue;

            masterTilemap.SetColliderType(cell, Tile.ColliderType.None);
            decayedCells.Add(cell);
        }

        if (decayedCells.Count > 0)
        {
            OnColumnDecayed?.Invoke(decayedCells);
        }
    }

    // Decays every tile currently placed in the tilemap in one pass, on player death.
    public void DecayAll()
    {
        isActive = false;
        decayedCells.Clear();

        BoundsInt bounds = masterTilemap.cellBounds;
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!masterTilemap.HasTile(cell)) continue;

            masterTilemap.SetColliderType(cell, Tile.ColliderType.None);
            decayedCells.Add(cell);
        }

        if (decayedCells.Count > 0)
        {
            OnColumnDecayed?.Invoke(decayedCells);
        }
    }

    public void BeginDecay()
    {
        isActive = true;
    }
}