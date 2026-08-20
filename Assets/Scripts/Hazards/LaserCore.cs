using BFTools.Core.EventBus;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserCore : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector3 startOffset = Vector3.zero;
    [SerializeField] private LaserType laserType;
    public LaserType LaserType => laserType;

    [Header("State")]
    [SerializeField] private bool laserEnabled = true;

    private LineRenderer lineRenderer;
    private Vector3 currentDirection;

    public Vector3 CurrentDirection => currentDirection;
    public bool IsLaserActive => laserEnabled;

    private LaserVisuals visuals;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;

        visuals = GetComponent<LaserVisuals>();
    }

    private void OnEnable()
    {
        currentDirection = Vector3.zero;
    }

    private void Update()
    {
        if (!laserEnabled)
        {
            if (lineRenderer.enabled) lineRenderer.enabled = false;
            return;
        }

        if (currentDirection == Vector3.zero)
        {
            currentDirection = transform.up;
        }

        Vector3 endPosition = FireRaycast();
        DrawLaser(startOffset, transform.InverseTransformDirection(endPosition));
        CheckPlayerCollision(endPosition);
    }

    public void SetDirection(Vector3 direction)
    {
        currentDirection = direction.normalized;
    }

    public void ToggleLaser(bool enabled)
    {
        laserEnabled = enabled;
        lineRenderer.enabled = enabled;

        if (visuals != null)
            visuals.ToggleParticles(enabled);
    }

    private Vector3 FireRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, Mathf.Infinity, collisionLayer);
        Vector3 endPosition = currentDirection * 1000f;

        if (hit.collider != null)
        {
            endPosition = hit.point - (Vector2)transform.position;
        }

        return endPosition;
    }

    private void DrawLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void CheckPlayerCollision(Vector3 endPosition)
    {
        float distance = endPosition.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, distance, playerLayer);
        if (hit.collider != null)
        {
            EventBus<PlayerDeathEvent>.Fire(new PlayerDeathEvent());
        }
    }
}

public enum LaserType
{
    Static,
    Slide,
    Pulse
}