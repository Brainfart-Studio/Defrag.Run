using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileDecayer))]
public class DecayFragmentSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap masterTilemap;

    private TileDecayer tileDecayer;
    private BFObjectPooler pooler;

    private void Awake()
    {
        tileDecayer = GetComponent<TileDecayer>();
    }

    private void OnEnable()
    {
        tileDecayer.OnTileDecayed += HandleTileDecayed;
    }

    private void OnDisable()
    {
        tileDecayer.OnTileDecayed -= HandleTileDecayed;
    }

    private void HandleTileDecayed(Vector3Int cell)
    {
        Sprite sprite = masterTilemap.GetSprite(cell);
        Vector3 worldPosition = masterTilemap.GetCellCenterWorld(cell);

        masterTilemap.SetTile(cell, null);

        if (sprite == null) return;

        if (pooler == null)
        {
            pooler = BFServiceLocator.Get<BFObjectPooler>();
        }

        GameObject instance = pooler.Get(DecayFragmentController.PoolKey);
        if (instance == null) return;

        instance.GetComponent<DecayFragmentController>().Begin(sprite, worldPosition);
    }
}