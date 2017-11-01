using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool IsOwnedByPlayer = false;
    public bool IsKing = false;

    [Header("King stuff")]
    public Transform CrownPrefab;
    
    public Vector2 Position { get; set; }

    public static Piece Create(
        Transform prefab, Vector3 worldPos, Transform parent, 
        bool isOwnedByPlayer, Vector2 boardPos)
    {
        var obj = Instantiate(prefab);
        obj.position = worldPos;
        obj.parent = parent;
        var piece = obj.GetComponent<Piece>();
        piece.IsOwnedByPlayer = isOwnedByPlayer;
        piece.Position = boardPos;
        //GameManager.Instance.Board.Pieces[(int)boardPos.x, (int)boardPos.y] = piece;
        return piece;
    }
    
    public void Highlight()
    {
        var pos = transform.position;
        pos.y = 1;
        transform.position = pos;
    }

    public void UnHighlight()
    {
        var pos = transform.position;
        pos.y = 0;
        transform.position = pos;
    }

    public void MoveTo(Vector2 newPos)
    {
        GameManager.Instance.Board.MovePiece(Position, newPos);
        Position = newPos;
        transform.position = GameManager.Instance.Grid.TilePositions[(int) Position.x, (int) Position.y];
        UnHighlight();

        if (!IsKing) CheckKingStatus();
    }

    private void CheckKingStatus()
    {
        IsKing = IsOwnedByPlayer && (int)Position.y == GameManager.Instance.Grid.GridHeight - 1 ||
                 !IsOwnedByPlayer && (int)Position.y == 0;

        if (IsKing)
        {
            Instantiate(CrownPrefab, transform);
        }
    }

    public override string ToString()
    {
        return IsOwnedByPlayer ? (IsKing ? "3" : "1") : (IsKing ? "4" : "2");
    }
}
