using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Piece[,] Pieces { get; set; }

    public List<Move> RequiredMoves { get; private set; }

    public void Init()
    {
        var grid = GameManager.Instance.Grid;
        Pieces = new Piece[grid.GridWidth, grid.GridHeight];
        RequiredMoves = new List<Move>();
    }

    public void ClearBoard()
    {
        foreach (var piece in Pieces)
        {
            if (piece == null) continue;

            Destroy(piece.gameObject);
            Pieces[(int) piece.Position.x, (int) piece.Position.y] = null;
        }
    }

    public void UpdateRequiredMoves()
    {
        RequiredMoves = BoardMoves.GetRequiredMoves(Pieces, GameManager.Instance.IsPlayerTurn);
    }

    public bool UpdateRequiredMoves(Piece piece)
    {
        RequiredMoves = BoardMoves.GetRequiredMoves(Pieces, piece, GameManager.Instance.IsPlayerTurn);
        return RequiredMoves.Any();
    }

    public List<Move> GetAllowedMoves(Piece piece, out bool isKill)
    {
        return BoardMoves.GetAllowedMoves(Pieces, piece, GameManager.Instance.IsPlayerTurn, out isKill);
    }

    public void MovePiece(Vector2 oldPos, Vector2 newPos)
    {
        var x = (int) oldPos.x;
        var y = (int) oldPos.y;
        var piece = Pieces[x, y];
        Pieces[x, y] = null;
        Pieces[(int) newPos.x, (int) newPos.y] = piece;
    }

    public void KillPiece(Vector2 pos)
    {
        var x = (int) pos.x;
        var y = (int) pos.y;
        var piece = Pieces[x, y];
        Destroy(piece.gameObject);
        Pieces[x, y] = null;
    }

    public void KillPiece(Piece piece)
    {
        KillPiece(piece.Position);
    }
}

public enum TileDirection
{
    Left,
    TopLeft,
    TopRight,
    Right,
    BottomRight,
    BottomLeft
}