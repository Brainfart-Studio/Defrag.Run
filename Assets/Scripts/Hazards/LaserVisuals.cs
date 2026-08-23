using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserVisuals : MonoBehaviour
{
    [Header("Jagged Settings")]
    [SerializeField] private float segmentLength = 0.125f;
    [SerializeField] private float jaggedness = 0.2f;

    [Header("Visual Settings")]
    [SerializeField] private Color laserColor = Color.red;
    [SerializeField] private GameObject laserParticles;

    private LineRenderer lineRenderer;
    private GameObject activeParticleSystem;

    public Color GetLaserColor() => lineRenderer.startColor;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ApplyColor();
    }

    private void LateUpdate()
    {
        if (!lineRenderer.enabled || lineRenderer.positionCount < 2)
        {
            ToggleParticles(false);
            return;
        }

        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(1);

        ApplyJaggedEffect(startPoint, endPoint);
        UpdateParticles(endPoint);
    }

    private void ApplyJaggedEffect(Vector3 startPoint, Vector3 endPoint)
    {
        float distance = Vector3.Distance(startPoint, endPoint);
        int segmentCount = Mathf.Max(1, Mathf.RoundToInt(distance / segmentLength));

        Vector3[] jaggedPoints = new Vector3[segmentCount + 1];
        jaggedPoints[0] = startPoint;
        jaggedPoints[segmentCount] = endPoint;

        Vector3 direction = (endPoint - startPoint).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

        for (int i = 1; i < segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t);
            point += perpendicular * Random.Range(-jaggedness, jaggedness);
            jaggedPoints[i] = point;
        }

        lineRenderer.positionCount = jaggedPoints.Length;
        lineRenderer.SetPositions(jaggedPoints);
    }

    private void ApplyColor()
    {
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;

        if (lineRenderer.material != null && lineRenderer.material.HasProperty("_Color"))
        {
            lineRenderer.material.SetColor("_Color", laserColor);
        }
    }

    private void UpdateParticles(Vector3 endPoint)
    {
        if (laserParticles == null) return;

        Vector3 worldEnd = transform.TransformPoint(endPoint);

        if (activeParticleSystem == null)
        {
            activeParticleSystem = Instantiate(laserParticles, worldEnd, Quaternion.identity);
            activeParticleSystem.transform.SetParent(transform);

            ParticleSystem ps = activeParticleSystem.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = laserColor;
            }
        }
        else
        {
            activeParticleSystem.transform.position = worldEnd;
        }

        activeParticleSystem.transform.rotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.z);

        ToggleParticles(true);
    }

    public void ToggleParticles(bool enabled)
    {
        if (activeParticleSystem == null) return;

        var ps = activeParticleSystem.GetComponent<ParticleSystem>();
        if (ps == null) return;

        if (enabled && !ps.isPlaying)
        {
            ps.Play();
        }
        else if (!enabled && ps.isPlaying)
        {
            ps.Stop();
        }
    }

    public Vector3[] GetJaggedPoints()
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);
        return points;
    }
}