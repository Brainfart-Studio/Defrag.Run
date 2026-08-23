using UnityEngine;

public static class CrumbleMeshBuilder
{
    // Each cell gets its own unshared vertices so per-cell shader displacement
    // (added in a later commit) can move a cell without tearing its neighbors apart.
    // Vertex color .r carries a stable per-cell random seed for that same shader to consume.
    public static Mesh Build(int cellsPerAxis, float size)
    {
        if (cellsPerAxis < 1) cellsPerAxis = 1;

        int cellCount = cellsPerAxis * cellsPerAxis;
        int vertexCount = cellCount * 4;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv0 = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];
        int[] triangles = new int[cellCount * 6];

        float cellSize = size / cellsPerAxis;
        float halfSize = size * 0.5f;

        int vertIndex = 0;
        int triIndex = 0;

        for (int y = 0; y < cellsPerAxis; y++)
        {
            for (int x = 0; x < cellsPerAxis; x++)
            {
                float xMin = x * cellSize - halfSize;
                float xMax = xMin + cellSize;
                float yMin = y * cellSize - halfSize;
                float yMax = yMin + cellSize;

                float uMin = x / (float)cellsPerAxis;
                float uMax = (x + 1) / (float)cellsPerAxis;
                float vMin = y / (float)cellsPerAxis;
                float vMax = (y + 1) / (float)cellsPerAxis;

                vertices[vertIndex + 0] = new Vector3(xMin, yMin, 0f);
                vertices[vertIndex + 1] = new Vector3(xMax, yMin, 0f);
                vertices[vertIndex + 2] = new Vector3(xMin, yMax, 0f);
                vertices[vertIndex + 3] = new Vector3(xMax, yMax, 0f);

                uv0[vertIndex + 0] = new Vector2(uMin, vMin);
                uv0[vertIndex + 1] = new Vector2(uMax, vMin);
                uv0[vertIndex + 2] = new Vector2(uMin, vMax);
                uv0[vertIndex + 3] = new Vector2(uMax, vMax);

                Color cellColor = new Color(Random.value, 0f, 0f, 1f);
                colors[vertIndex + 0] = cellColor;
                colors[vertIndex + 1] = cellColor;
                colors[vertIndex + 2] = cellColor;
                colors[vertIndex + 3] = cellColor;

                triangles[triIndex + 0] = vertIndex + 0;
                triangles[triIndex + 1] = vertIndex + 2;
                triangles[triIndex + 2] = vertIndex + 1;
                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + 2;
                triangles[triIndex + 5] = vertIndex + 3;

                vertIndex += 4;
                triIndex += 6;
            }
        }

        Mesh mesh = new Mesh { name = $"CrumbleMesh_{cellsPerAxis}x{cellsPerAxis}" };
        mesh.vertices = vertices;
        mesh.uv = uv0;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        return mesh;
    }
}