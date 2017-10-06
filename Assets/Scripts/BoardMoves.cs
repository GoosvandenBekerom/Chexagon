using System;
using System.Collections.Generic;
using UnityEngine;

public static class BoardMoves
{
    public static List<Move> GetRequiredMoves(Piece[,] board, bool isPlayerTurn)
    {
        var required = new List<Move>();
        foreach (var piece in board)
        {
            if (piece == null || piece.IsOwnedByPlayer != isPlayerTurn) continue;

            bool isKill;
            var moves = GetAllowedMoves(board, piece, isPlayerTurn, out isKill);
            if (isKill) required.AddRange(moves);
        }
        return required;
    }

    public static List<Move> GetRequiredMoves(Piece[,] board, Piece piece, bool isPlayerTurn)
    {
        var required = new List<Move>();
        if (piece == null || piece.IsOwnedByPlayer != isPlayerTurn) return required;

        bool isKill;
        var moves = GetAllowedMoves(board, piece, isPlayerTurn, out isKill);
        if (isKill)
        {
            required = moves;
        }

        return required;
    }

    public static List<Move> GetAllowedMoves(Piece[,] board, Piece piece, bool isPlayerTurn, out bool isKill)
    {
        var adjacentTiles = GetAdjacentTiles(board, piece);

        var allowed = new List<Move>();
        var required = new List<Move>();

        foreach (var kvp in adjacentTiles)
        {
            var tile = kvp.Key;
            var dir = kvp.Value;
            int x = (int)tile.x, y = (int)tile.y;

            var adjacent = board[x, y];
            if (adjacent == null) allowed.Add(new Move(piece, tile));
            else if (adjacent.IsOwnedByPlayer != isPlayerTurn)
            {
                // check if it is possible to kill the adjacent piece
                var killLocation = GetAdjacentPiece(board, adjacent, dir);
                if (killLocation.Key != null) continue;

                var pos = killLocation.Value;
                if (InBounds(board, pos)) required.Add(new Move(piece, pos, true, adjacent));
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

    public static Dictionary<Vector2, TileDirection> GetAdjacentTiles(Piece[,] board, Piece piece)
    {
        var adjacentTiles = new Dictionary<Vector2, TileDirection>();
        var x = (int)piece.Position.x;
        var y = (int)piece.Position.y;

        var left = new Vector2(x - 1, y);
        var right = new Vector2(x + 1, y);
        if (InBounds(board, left)) adjacentTiles.Add(left, TileDirection.Left);
        if (InBounds(board, right)) adjacentTiles.Add(right, TileDirection.Right);

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
        if (InBounds(board, topLeft) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topLeft, TileDirection.TopLeft);
        if (InBounds(board, topRight) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topRight, TileDirection.TopRight);
        if (InBounds(board, botRight) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botRight, TileDirection.BottomRight);
        if (InBounds(board, botLeft) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botLeft, TileDirection.BottomLeft);

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
    public static KeyValuePair<Piece, Vector2> GetAdjacentPiece(Piece[,] board, Piece piece, TileDirection direction)
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
        return InBounds(board, tile)
            ? new KeyValuePair<Piece, Vector2>(board[x2, y2], pos)
            : new KeyValuePair<Piece, Vector2>(null, pos);
    }
    
    public static bool InBounds(Piece[,] board, Vector2 p)
    {
        var maxX = board.GetLength(0);
        var maxY = board.GetLength(1);
        return (p.x >= 0 && p.x < maxX && p.y >= 0 && p.y < maxY);
    }
}
