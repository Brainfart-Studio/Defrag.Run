using System.Collections.Generic;
using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileBuilder))]
public class BuildFragmentSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap masterTilemap;

    [Tooltip("Collider type to restore once a tile finishes assembling. Must match the tile assets' configured collider type")]
    [SerializeField] private Tile.ColliderType restoredColliderType = Tile.ColliderType.Sprite;

    private TileBuilder tileBuilder;
    private BFObjectPooler pooler;

    private void Awake()
    {
        tileBuilder = GetComponent<TileBuilder>();
    }

    private void OnEnable()
    {
        tileBuilder.OnColumnBuilding += HandleColumnBuilding;
    }

    private void OnDisable()
    {
        tileBuilder.OnColumnBuilding -= HandleColumnBuilding;
    }

    private void HandleColumnBuilding(IReadOnlyList<Vector3Int> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Vector3Int cell = cells[i];
            Sprite sprite = masterTilemap.GetSprite(cell);
            if (sprite == null) continue;

            Vector3 worldPosition = masterTilemap.GetCellCenterWorld(cell);

            if (pooler == null)
            {
                pooler = BFServiceLocator.Get<BFObjectPooler>();
            }

            GameObject instance = pooler.Get(BuildFragmentController.PoolKey);
            if (instance == null) continue;

            instance.GetComponent<BuildFragmentController>().Begin(sprite, worldPosition, () => RevealTile(cell));
        }
    }

    // Called by a fragment's onComplete once its assembly animation finishes -
    // the tile only becomes visible and solid at the moment it visually looks
    // fully assembled, not the moment the build line first reached it.
    private void RevealTile(Vector3Int cell)
    {
        masterTilemap.SetTileFlags(cell, TileFlags.None);
        masterTilemap.SetColor(cell, Color.white);
        masterTilemap.SetColliderType(cell, restoredColliderType);
    }
}