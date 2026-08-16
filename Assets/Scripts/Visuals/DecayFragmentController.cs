using BFTools.Core.ServiceLocator;
using BFTools.Systems.ObjectPooler;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DecayFragmentController : MonoBehaviour
{
    public const string PoolKey = "DecayFragment";

    private static readonly int DecayProgressId = Shader.PropertyToID("_DecayProgress");
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
    private bool isDecaying;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        meshFilter.mesh = CrumbleMeshBuilder.Build(cellsPerAxis, tileSize);
    }

    private void Update()
    {
        if (!isDecaying) return;

        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / lifetime);

        propertyBlock.SetFloat(DecayProgressId, progress);
        meshRenderer.SetPropertyBlock(propertyBlock);

        if (progress >= 1f)
        {
            ReturnToPool();
        }
    }

    // Starts this pooled instance decaying a specific tile sprite at a world position.
    // Remaps _MainTex_ST to the sprite's packed rect since tile sprites come from the
    // auto-tile rule tile atlas, not a standalone 0-1 texture.
    public void Begin(Sprite sprite, Vector3 worldPosition)
    {
        transform.position = worldPosition;
        elapsed = 0f;
        isDecaying = true;

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
        propertyBlock.SetFloat(DecayProgressId, 0f);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void ReturnToPool()
    {
        isDecaying = false;

        if (pooler == null)
        {
            pooler = BFServiceLocator.Get<BFObjectPooler>();
        }

        pooler.Release(gameObject);
    }
}