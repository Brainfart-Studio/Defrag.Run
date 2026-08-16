using System.Collections.Generic;
using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileDecayer))]
public class DecayFragmentSpawner : MonoBehaviour
{
    private struct DecayedTile
    {
        public Vector3Int cell;
        public Sprite sprite;
        public Vector3 worldPosition;
    }

    [SerializeField] private Tilemap masterTilemap;

    private TileDecayer tileDecayer;
    private BFObjectPooler pooler;
    private readonly List<DecayedTile> decayedTiles = new List<DecayedTile>();

    private void Awake()
    {
        tileDecayer = GetComponent<TileDecayer>();
    }

    private void OnEnable()
    {
        tileDecayer.OnColumnDecayed += HandleColumnDecayed;
    }

    private void OnDisable()
    {
        tileDecayer.OnColumnDecayed -= HandleColumnDecayed;
    }

    private void HandleColumnDecayed(IReadOnlyList<Vector3Int> cells)
    {
        decayedTiles.Clear();

        // Read every sprite before clearing any tile. Clearing one cell triggers
        // a rule-tile neighbor refresh, so reading after a clear would capture an
        // already-eroded sprite instead of the tile's true connected appearance.
        for (int i = 0; i < cells.Count; i++)
        {
            Vector3Int cell = cells[i];
            decayedTiles.Add(new DecayedTile
            {
                cell = cell,
                sprite = masterTilemap.GetSprite(cell),
                worldPosition = masterTilemap.GetCellCenterWorld(cell)
            });
        }

        for (int i = 0; i < decayedTiles.Count; i++)
        {
            masterTilemap.SetTile(decayedTiles[i].cell, null);
        }

        for (int i = 0; i < decayedTiles.Count; i++)
        {
            DecayedTile tile = decayedTiles[i];
            if (tile.sprite == null) continue;

            if (pooler == null)
            {
                pooler = BFServiceLocator.Get<BFObjectPooler>();
            }

            GameObject instance = pooler.Get(DecayFragmentController.PoolKey);
            if (instance == null) continue;

            instance.GetComponent<DecayFragmentController>().Begin(tile.sprite, tile.worldPosition);
        }
    }
}