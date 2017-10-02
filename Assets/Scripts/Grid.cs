using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid Prefabs")]
    public Transform HexagonPrefab;

    [Header("Grid Size")]
    [Range(5, 11)]
    public int GridWidth = 9;
    [Range(5, 11)]
    public int GridHeight = 9;

    private float hexWidth = 1.732f;
    private float hexHeight = 2.0f;

    [Header("Grid Information")]
    [Range(0.1f, 0.25f)]
    public float gap = 0.1f;

    private Vector3 startPos;

    public Vector3[,] TilePositions { get; private set; }

    public void Generate()
    {
        TilePositions = new Vector3[GridWidth, GridHeight];

        AddGap();
        CalcStartPos();
        CreateGrid();
    }

    private void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    private void CalcStartPos()
    {
        var offset = 0.0f;
        if (GridHeight / 2 % 2 != 0) offset = hexWidth / 2;

        var x = -hexWidth * (GridWidth / 2) - offset;
        var z = -hexHeight * 0.75f * (GridHeight / 2);

        startPos = new Vector3(x, 0, z);
    }

    private Vector3 CalcWorldPos(Vector2 gridPos)
    {
        var offset = 0.0f;
        if (gridPos.y % 2 != 0) offset = hexWidth / 2;

        var x = startPos.x + gridPos.x * hexWidth + offset;
        var z = startPos.z + gridPos.y * hexHeight * 0.75f;

        return new Vector3(x, 0, z);
    }

    private void CreateGrid()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                var hex = Instantiate(HexagonPrefab);
                var worldPos = CalcWorldPos(new Vector2(x, y));
                hex.position = worldPos;
                hex.parent = transform;
                hex.name = "Hexagon " + x + "|" + y;

                TilePositions[x, y] = worldPos;
            }
        }
    }
}
