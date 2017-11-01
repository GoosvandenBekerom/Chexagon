using UnityEngine;

public class Move
{
    public Piece Piece { get; private set; }
    public Vector2 Destination { get; private set; }
    public bool IsKill { get; private set; }
    public Piece Target { get; private set; }

    public Move(Piece piece, Vector2 dest, bool isKill = false, Piece target = null)
    {
        Piece = piece;
        Destination = dest;
        IsKill = isKill;
        Target = target;
    }

    public void Execute()
    {
        Piece.MoveTo(Destination);
        if (IsKill) GameManager.Instance.Board.KillPiece(Target);
    }

    public override string ToString()
    {
        return "Origin: x=" + Piece.Position.x + ", y=" + Piece.Position.y +
               " - Destination: x=" + Destination.x + ", y=" + Destination.y +
               " - Is Kill?: " + IsKill +
               " - Target Position: x=" + Target.Position.x + ", y=" + Target.Position.y;
    }
}
