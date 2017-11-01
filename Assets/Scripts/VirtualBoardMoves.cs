using System;
using System.Collections.Generic;

public static class VirtualBoardMoves
{
    public static List<VirtualMove> GetRequiredMoves(int[,] board, bool isPlayerTurn)
    {
        var required = new List<VirtualMove>();

        for (var x = 0; x < board.GetLength(0); x++)
        {
            for (var y = 0; y < board.GetLength(0); y++)
            {
                var piece = board[x, y];
                if (piece == BoardOccupation.PLAYER_NONE) continue;
                if (isPlayerTurn != (piece == BoardOccupation.PLAYER_1 || piece == BoardOccupation.PLAYER_1_KING)) continue;

                bool isKill;
                var moves = GetAllowedMoves(board, new VirtualTile(x, y), isPlayerTurn, out isKill);
                if (isKill) required.AddRange(moves);
            }
        }

        return required;
    }

    public static List<VirtualMove> GetAllowedMoves(int[,] board, VirtualTile originTile, bool isPlayerTurn, out bool isKill)
    {
        var adjacentTiles = GetAdjacentTiles(board, originTile);
        var player = isPlayerTurn ? BoardOccupation.PLAYER_1 : BoardOccupation.PLAYER_2;
        var playerKing = isPlayerTurn ? BoardOccupation.PLAYER_1_KING : BoardOccupation.PLAYER_2_KING;

        var allowed = new List<VirtualMove>();
        var required = new List<VirtualMove>();

        foreach (var kvp in adjacentTiles)
        {
            var adjacentTile = kvp.Key;
            var dir = kvp.Value;

            var adjacentOwner = board[adjacentTile.x, adjacentTile.y];
            if (adjacentOwner == BoardOccupation.PLAYER_NONE)
            {
                allowed.Add(new VirtualMove(originTile, adjacentTile));
            }
            else if (adjacentOwner != player && adjacentOwner != playerKing)
            {
                // check if it is possible to kill the adjacent piece
                var killLocation = GetAdjacentPiece(board, adjacentTile, dir);
                if (killLocation.Key == -1 || killLocation.Key != BoardOccupation.PLAYER_NONE) continue;

                var killPos = killLocation.Value;
                if (InBounds(board, killPos)) required.Add(new VirtualMove(originTile, killPos, true, adjacentTile));
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

    public static Dictionary<VirtualTile, TileDirection> GetAdjacentTiles(int[,] board, VirtualTile tile)
    {
        var adjacentTiles = new Dictionary<VirtualTile, TileDirection>();
        var x = tile.x;
        var y = tile.y;

        var left = new VirtualTile(x - 1, y);
        var right = new VirtualTile(x + 1, y);
        if (InBounds(board, left)) adjacentTiles.Add(left, TileDirection.Left);
        if (InBounds(board, right)) adjacentTiles.Add(right, TileDirection.Right);

        // Even row
        var topLeft = new VirtualTile(x - 1, y + 1);
        var topRight = new VirtualTile(x, y + 1);
        var botRight = new VirtualTile(x, y - 1);
        var botLeft = new VirtualTile(x - 1, y - 1);

        if (y % 2 != 0)
        {
            // Odd row
            topLeft.x += 1;
            topRight.x += 1;
            botRight.x += 1;
            botLeft.x += 1;
        }

        var piece = board[x, y];
        var isKing = piece == BoardOccupation.PLAYER_1_KING || piece == BoardOccupation.PLAYER_2_KING;

        // Add all tiles the player is allowed to move to
        if (InBounds(board, topLeft) && (piece == BoardOccupation.PLAYER_1 || isKing)) adjacentTiles.Add(topLeft, TileDirection.TopLeft);
        if (InBounds(board, topRight) && (piece == BoardOccupation.PLAYER_1 || isKing)) adjacentTiles.Add(topRight, TileDirection.TopRight);
        if (InBounds(board, botRight) && (piece == BoardOccupation.PLAYER_2 || isKing)) adjacentTiles.Add(botRight, TileDirection.BottomRight);
        if (InBounds(board, botLeft) && (piece == BoardOccupation.PLAYER_2 || isKing)) adjacentTiles.Add(botLeft, TileDirection.BottomLeft);

        return adjacentTiles;
    }
    
    public static KeyValuePair<int, VirtualTile> GetAdjacentPiece(int[,] board, VirtualTile originTile, TileDirection direction)
    {
        VirtualTile tile;
        var x = originTile.x;
        var y = originTile.y;
        var odd = y % 2 != 0;

        switch (direction)
        {
            case TileDirection.Left:
                tile = new VirtualTile(x - 1, y);
                break;
            case TileDirection.TopLeft:
                tile = new VirtualTile(x - 1, y + 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.TopRight:
                tile = new VirtualTile(x, y + 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.Right:
                tile = new VirtualTile(x + 1, y);
                //possible = true;
                break;
            case TileDirection.BottomRight:
                tile = new VirtualTile(x, y - 1);
                if (odd) tile.x += 1;
                break;
            case TileDirection.BottomLeft:
                tile = new VirtualTile(x - 1, y - 1);
                if (odd) tile.x += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
        }
        
        var pos = new VirtualTile(tile.x, tile.y);
        return InBounds(board, tile)
            ? new KeyValuePair<int, VirtualTile>(board[tile.x, tile.y], pos)
            : new KeyValuePair<int, VirtualTile>(-1, pos);
    }

    public static bool InBounds(int[,] board, VirtualTile p)
    {
        var maxX = board.GetLength(0);
        var maxY = board.GetLength(1);
        return (p.x >= 0 && p.x < maxX && p.y >= 0 && p.y < maxY);
    }
}

public static class BoardOccupation
{
    public const int PLAYER_NONE = 0;
    public const int PLAYER_1 = 1;
    public const int PLAYER_2 = 2; 
    public const int PLAYER_1_KING = 3;
    public const int PLAYER_2_KING = 4;
}

public static class BoardStateScores
{
    public const int WIN = 100000;
    public const int PIECE_NORMAL = 10;
    public const int PIECE_KING = 25;
    public const int KILL = 100;
    public const int DEFENCE = 100;
}

public class VirtualTile
{
    public int x;
    public int y;

    public VirtualTile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "x=" + x + ", y=" + y;
    }
}

public class VirtualMove
{
    public VirtualTile Origin { get; private set; }
    public VirtualTile Destination { get; private set; }
    public bool IsKill { get; private set; }
    public VirtualTile KillLocation { get; private set; }

    public VirtualMove(VirtualTile origin, VirtualTile destination, bool isKill = false, VirtualTile killLocation = null)
    {
        Origin = origin;
        Destination = destination;
        IsKill = isKill;
        KillLocation = killLocation;
    }

    public override string ToString()
    {
        return "Origin: " + Origin + ", Destination: " + Destination + ", Is Kill?: " + IsKill + ", Target location: " +
               KillLocation;
    }
}
