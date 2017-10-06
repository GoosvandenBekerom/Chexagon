using UnityEngine;

public class PiecesSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public Transform Piece1Prefab;
    public Transform Piece2Prefab;

    [Header("Options")]
    [Range(1, 3)]
    public int InitialRows = 2;

    public void Spawn()
    {
        var grid = GameManager.Instance.Grid;
        var height = grid.GridHeight;
        var width = grid.GridWidth;
        var tilePositions = grid.TilePositions;
        var boardState = GameManager.Instance.Board.Pieces;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (x % 2 != 0 && (y == 0 || y == height - 1)) continue;

                if (y < InitialRows)
                {
                    boardState[x, y] = Piece.Create(Piece1Prefab, tilePositions[x, y], transform, true, new Vector2(x, y));
                }
                else if (y > height - (InitialRows + 1))
                {
                    boardState[x, y] = Piece.Create(Piece2Prefab, tilePositions[x, y], transform, false, new Vector2(x, y));
                }
            }
        }
    }
}
