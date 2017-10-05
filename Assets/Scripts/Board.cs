using System;
using System.Collections.Generic;
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

    public void UpdateRequiredMoves()
    {
        RequiredMoves.Clear();
        foreach (var piece in Pieces)
        {
            if (piece == null || piece.IsOwnedByPlayer != GameManager.Instance.IsPlayerTurn) continue;
            
            bool isKill;
            var moves = GetAllowedMoves(piece, out isKill);
            if (isKill) RequiredMoves.AddRange(moves);
        }
    }

    public bool UpdateRequiredMoves(Piece piece)
    {
        RequiredMoves.Clear();

        if (piece == null || piece.IsOwnedByPlayer != GameManager.Instance.IsPlayerTurn) return false;

        bool isKill;
        var moves = GetAllowedMoves(piece, out isKill);
        if (isKill)
        {
            RequiredMoves.AddRange(moves);
            GameManager.Instance.Grid.HighlightRequiredTiles(RequiredMoves);
        }

        return isKill;
    }

    public List<Move> GetAllowedMoves(Piece piece, out bool isKill)
    {
        var adjacentTiles = GetAdjacentTiles(piece);

        var allowed = new List<Move>();
        var required = new List<Move>();

        foreach (var kvp in adjacentTiles)
        {
            var tile = kvp.Key;
            var dir = kvp.Value;
            int x = (int) tile.x, y = (int) tile.y;

            var adjacent = Pieces[x, y];
            if (adjacent == null) allowed.Add(new Move(piece, tile));
            else if (adjacent.IsOwnedByPlayer != GameManager.Instance.IsPlayerTurn)
            {
                // check if it is possible to kill the adjacent piece
                var killLocation = GetAdjacentPiece(adjacent, dir);
                if (killLocation.Key != null) continue;

                var pos = killLocation.Value;
                if (InBounds(pos)) required.Add(new Move(piece, pos, true, adjacent));
            }
        }

        if (required.Count > 0)
        {
            isKill = true;
            return required;
        }

        isKill = false;
        return allowed;
    }

    private Dictionary<Vector2, TileDirection> GetAdjacentTiles(Piece piece)
    {
        var adjacentTiles = new Dictionary<Vector2, TileDirection>();
        var x = (int)piece.Position.x;
        var y = (int)piece.Position.y;

        var left = new Vector2(x - 1, y);
        var right = new Vector2(x + 1, y);
        if (InBounds(left)) adjacentTiles.Add(left, TileDirection.Left);
        if (InBounds(right)) adjacentTiles.Add(right, TileDirection.Right);

        // Even row
        var topLeft = new Vector2(x - 1, y + 1);
        var topRight = new Vector2(x, y + 1);
        var botRight = new Vector2(x, y - 1);
        var botLeft = new Vector2(x - 1, y - 1);

        if (y % 2 != 0)
        {
            // Odd row
            topLeft.x += 1;
            topRight.x += 1;
            botRight.x += 1;
            botLeft.x += 1;
        }

        // Add all tiles the player is allowed to move to
        if (InBounds(topLeft) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topLeft, TileDirection.TopLeft);
        if (InBounds(topRight) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topRight, TileDirection.TopRight);
        if (InBounds(botRight) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botRight, TileDirection.BottomRight);
        if (InBounds(botLeft) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botLeft, TileDirection.BottomLeft);

        return adjacentTiles;
    }

    /// <summary>
    /// Get the adjacent piece in the given direction
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="direction"></param>
    /// <returns>
    /// Pair of adjacent piece in the given direction (null if tile is empty or out of bounds), and its position
    /// </returns>
    private KeyValuePair<Piece, Vector2> GetAdjacentPiece(Piece piece, TileDirection direction)
    {
        Vector2 tile;
        var x = (int)piece.Position.x;
        var y = (int)piece.Position.y;
        var odd = y % 2 != 0;

        switch (direction)
        {
            case TileDirection.Left:
                tile = new Vector2(x - 1, y);
                break;
            case TileDirection.TopLeft:
                tile = new Vector2(x - 1, y + 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.TopRight:
                tile = new Vector2(x, y + 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.Right:
                tile = new Vector2(x + 1, y);
                //possible = true;
                break;
            case TileDirection.BottomRight:
                tile = new Vector2(x, y - 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.BottomLeft:
                tile = new Vector2(x - 1, y - 1);
                if (odd) tile.x += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
        }

        int x2 = (int)tile.x, y2 = (int)tile.y;
        var pos = new Vector2(x2, y2);
        return InBounds(tile) 
            ? new KeyValuePair<Piece, Vector2>(Pieces[x2, y2], pos) 
            : new KeyValuePair<Piece, Vector2>(null, pos);
    }

    private bool InBounds(Vector2 p)
    {
        var maxX = Pieces.GetLength(0);
        var maxY = Pieces.GetLength(1);
        return (p.x >= 0 && p.x < maxX && p.y >= 0 && p.y < maxY);
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