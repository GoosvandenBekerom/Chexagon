using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool IsOwnedByPlayer = false;
    
    public Vector2 Position { get; set; }
    
    public static Piece Create(Transform prefab, Vector3 worldPos, Transform parent, bool isOwnedByPlayer, Vector2 boardPos)
    {
        var obj = Instantiate(prefab);
        obj.position = worldPos;
        obj.parent = parent;
        var piece = obj.GetComponent<Piece>();
        piece.IsOwnedByPlayer = isOwnedByPlayer;
        piece.Position = boardPos;
        return piece;
    }
}
