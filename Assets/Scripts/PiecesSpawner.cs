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
        var height = GameManager.Instance.Grid.GridHeight;
        var width = GameManager.Instance.Grid.GridWidth;
        var tilePositions = GameManager.Instance.Grid.TilePositions;
        var boardState = GameManager.Instance.Board.State;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (y < InitialRows)
                {
                    var piece = Instantiate(Piece1Prefab);
                    piece.position = tilePositions[x, y];
                    piece.parent = transform;

                    var pieceScript = piece.GetComponent<Piece>();
                    pieceScript.IsOwnedByPlayer = true;
                    pieceScript.Position = new Vector2(x, y);

                    boardState[x, y] = (int) TileState.Player1;
                }
                else if (y > height - (InitialRows + 1))
                {
                    var piece = Instantiate(Piece2Prefab);
                    piece.position = tilePositions[x, y];
                    piece.parent = transform;

                    var pieceScript = piece.GetComponent<Piece>();
                    pieceScript.Position = new Vector2(x, y);

                    boardState[x, y] = (int) TileState.Player2;
                }
            }
        }
    }
}
