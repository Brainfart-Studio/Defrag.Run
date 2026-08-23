using UnityEngine;

public class SlidingLaser : MonoBehaviour
{
    [SerializeField] private Vector2 relativeOffset = new Vector2();
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private bool moveBackAndForth = false;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float distance;
    private float startTime;
    private bool movingToTarget = true;

    // Reset here rather than Start - this is a pooled hazard reused across chunk
    // spawns via SetActive, so Start would only fire once and every later reuse
    // would resume mid-slide from wherever it stopped last time, or stay disabled
    // forever if a non-back-and-forth slide had already finished and turned itself off.
    private void OnEnable()
    {
        startPosition = transform.position;
        targetPosition = startPosition + relativeOffset;
        distance = Vector2.Distance(startPosition, targetPosition);
        startTime = Time.time;
        movingToTarget = true;
    }

    private void Update()
    {
        MoveLaser();
    }

    private void MoveLaser()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfDistance = distanceCovered / distance;

        Vector2 newPosition = movingToTarget
            ? Vector2.Lerp(startPosition, targetPosition, fractionOfDistance)
            : Vector2.Lerp(targetPosition, startPosition, fractionOfDistance);

        transform.position = newPosition;

        if (fractionOfDistance >= 1f)
        {
            if (moveBackAndForth)
            {
                movingToTarget = !movingToTarget;
                startTime = Time.time;
            }
            else
            {
                enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 previewStart = Application.isPlaying ? startPosition : transform.position;
        Vector2 previewEnd = previewStart + relativeOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(previewStart, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(previewEnd, 0.1f);
    }
}