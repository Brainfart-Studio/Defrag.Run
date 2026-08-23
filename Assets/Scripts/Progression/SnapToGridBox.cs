using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class SnapToGridBox : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 cellOffset = new Vector2(0.5f, 0.5f);

#if UNITY_EDITOR
    // Resizing with the Rect tool drags transform.localScale (and localPosition, to
    // keep the dragged edge in place) rather than the BoxCollider2D's own size/offset -
    // so this snaps the transform itself, same live-Update reasoning as SnapToGrid:
    // a Rect tool drag never goes through OnValidate.
    private void Update()
    {
        if (Application.isPlaying) return;

        Vector3 scale = transform.localScale;
        scale.x = SnapScaleAxis(scale.x);
        scale.y = SnapScaleAxis(scale.y);
        transform.localScale = scale;

        // A box spanning an odd cell count centers on a cell (half-integer, the
        // cellOffset case) but an even count centers on the cell boundary between
        // two cells (a whole integer) - so the position offset has to flip with
        // parity instead of always using cellOffset, or even widths land half a
        // unit off the grid.
        Vector3 position = transform.localPosition;
        position.x = SnapPositionAxis(position.x, GetParityOffset(scale.x, cellOffset.x));
        position.y = SnapPositionAxis(position.y, GetParityOffset(scale.y, cellOffset.y));
        transform.localPosition = position;
    }

    private float SnapScaleAxis(float value)
    {
        float sign = value < 0 ? -1f : 1f;
        float snapped = Mathf.Round(Mathf.Abs(value) / cellSize) * cellSize;
        return sign * Mathf.Max(cellSize, snapped);
    }

    private float GetParityOffset(float snappedScale, float oddOffset)
    {
        int cellCount = Mathf.RoundToInt(Mathf.Abs(snappedScale) / cellSize);
        return cellCount % 2 == 0 ? 0f : oddOffset;
    }

    private float SnapPositionAxis(float value, float offset)
    {
        return Mathf.Round((value - offset) / cellSize) * cellSize + offset;
    }
#endif
}