using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SnapToGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 cellOffset = new Vector2(0.5f, 0.5f);

#if UNITY_EDITOR
    // Runs every editor tick (not just on Inspector edits) so dragging this object
    // with the Scene view move gizmo snaps live - a gizmo drag sets transform.position
    // directly and never goes through OnValidate. Placed inside a chunk prefab, this
    // keeps hand-placed hazards locked to the tile grid instead of a fraction off.
    private void Update()
    {
        if (Application.isPlaying) return;

        Vector3 position = transform.localPosition;
        position.x = SnapAxis(position.x, cellOffset.x);
        position.y = SnapAxis(position.y, cellOffset.y);
        transform.localPosition = position;
    }

    private float SnapAxis(float value, float offset)
    {
        return Mathf.Round((value - offset) / cellSize) * cellSize + offset;
    }
#endif
}