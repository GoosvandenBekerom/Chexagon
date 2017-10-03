using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Piece[,] Pieces { get; set; }

    public void Init()
    {
        var grid = GameManager.Instance.Grid;
        Pieces = new Piece[grid.GridWidth, grid.GridHeight];
    }

    public List<Vector2> GetAllowedMoves(Piece piece)
    {
        var adjacentTiles = GetAdjacentTiles(piece);

        var allowed = new List<Vector2>();
        var required = new List<Vector2>();

        foreach (var tile in adjacentTiles)
        {
            var adjacent = Pieces[(int) tile.x, (int) tile.y];
            if (adjacent == null) allowed.Add(tile);
            else if (!adjacent.IsOwnedByPlayer) required.Add(tile);
        }

        return required.Count == 0 ? allowed : required;
    }

    private List<Vector2> GetAdjacentTiles(Piece piece)
    {
        var adjacentTiles = new List<Vector2>();
        var x = (int)piece.Position.x;
        var y = (int)piece.Position.y;

        var left = new Vector2(x - 1, y);
        var right = new Vector2(x + 1, y);
        if (InBounds(left)) adjacentTiles.Add(left);
        if (InBounds(right)) adjacentTiles.Add(right);

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
        if (InBounds(topLeft) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topLeft);
        if (InBounds(topRight) && (piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(topRight);
        if (InBounds(botRight) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botRight);
        if (InBounds(botLeft) && (!piece.IsOwnedByPlayer || piece.IsKing)) adjacentTiles.Add(botLeft);

        return adjacentTiles;
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
}