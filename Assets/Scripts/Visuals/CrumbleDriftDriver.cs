using UnityEngine;

public class CrumbleDriftDriver : MonoBehaviour
{
    private static readonly int BackwardDriftId = Shader.PropertyToID("_BackwardDrift");

    [SerializeField] private CameraScroller cameraScroller;
    [SerializeField] private Material crumbleMaterial;

    [Tooltip("Multiplier applied to the camera's current scroll speed to get the backward drift amount")]
    [SerializeField] private float driftScale = 0.15f;

    private void Update()
    {
        crumbleMaterial.SetFloat(BackwardDriftId, cameraScroller.CurrentSpeed * driftScale);
    }
}