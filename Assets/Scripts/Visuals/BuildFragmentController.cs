using System;
using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BuildFragmentController : MonoBehaviour
{
    public const string PoolKey = "BuildFragment";

    private static readonly int BuildProgressId = Shader.PropertyToID("_BuildProgress");
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int MainTexStId = Shader.PropertyToID("_MainTex_ST");

    [SerializeField] private int cellsPerAxis = 4;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float lifetime = 0.6f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;
    private BFObjectPooler pooler;

    private float elapsed;
    private bool isBuilding;
    private Action onComplete;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        meshFilter.mesh = CrumbleMeshBuilder.Build(cellsPerAxis, tileSize);
    }

    private void Update()
    {
        if (!isBuilding) return;

        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / lifetime);

        propertyBlock.SetFloat(BuildProgressId, progress);
        meshRenderer.SetPropertyBlock(propertyBlock);

        if (progress >= 1f)
        {
            Complete();
        }
    }

    // Starts this pooled instance assembling a specific tile sprite at a world
    // position. onComplete fires once the assembly animation finishes, right
    // before this instance releases back to the pool - the caller uses that to
    // reveal the real tile and enable its collider at the exact moment the
    // fragment finishes looking solid, not the moment it started.
    public void Begin(Sprite sprite, Vector3 worldPosition, Action onComplete)
    {
        transform.position = worldPosition;
        elapsed = 0f;
        isBuilding = true;
        this.onComplete = onComplete;

        Texture texture = sprite.texture;
        Rect rect = sprite.rect;
        Vector4 uvRect = new Vector4(
            rect.width / texture.width,
            rect.height / texture.height,
            rect.x / texture.width,
            rect.y / texture.height);

        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture(MainTexId, texture);
        propertyBlock.SetVector(MainTexStId, uvRect);
        propertyBlock.SetFloat(BuildProgressId, 0f);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void Complete()
    {
        isBuilding = false;

        Action callback = onComplete;
        onComplete = null;
        callback?.Invoke();

        if (pooler == null)
        {
            pooler = BFServiceLocator.Get<BFObjectPooler>();
        }

        pooler.Release(gameObject);
    }
}