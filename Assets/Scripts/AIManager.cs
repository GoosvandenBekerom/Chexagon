using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    public void DoTurnRandom()
    {
        var pieces = GameManager.Instance.Board.Pieces;
        var moves = new List<Move>();
        var required = GameManager.Instance.Board.RequiredMoves;
        while (true)
        {
            Move move;
            if (required.Any())
            {
                move = required[Random.Range(0, required.Count)];
            }
            else
            {
                var killingMoves = new List<Move>();
                foreach (var piece in pieces)
                {
                    if (piece == null || piece.IsOwnedByPlayer) continue;

                    bool isKill;
                    var allowed = BoardMoves.GetAllowedMoves(pieces, piece, false, out isKill);

                    if (isKill) killingMoves.AddRange(allowed);
                    else moves.AddRange(allowed);
                }

                move = killingMoves.Any() 
                    ? killingMoves[Random.Range(0, killingMoves.Count)] 
                    : moves[Random.Range(0, moves.Count)];
            }
            move.Execute();

            if (!move.IsKill) break;

            if (GameManager.Instance.Board.UpdateRequiredMoves(move.Piece))
            {
                required = GameManager.Instance.Board.RequiredMoves;
                continue;
            }

            break;
        }
        GameManager.Instance.EndTurn();
    }

    public void DoTurn()
    {
        var boardState = ConvertToVirtualBoard(GameManager.Instance.Board.Pieces);
        var root = new GameNode(boardState);
        //PrintBoard(root.BoardState);
        const int depth = 3;

        BuildTree(root, depth, GameManager.Instance.IsPlayerTurn);

        var bestvalue = Minimax(root, depth, GameManager.Instance.IsPlayerTurn);

        // find the move associated with this value
        var potentialMoves = (from child in root.Children let node = child.Key
                              where node.GetHeuristic() == bestvalue
                              select CreateMoveFromVirtualMove(child.Value)).ToList();

        potentialMoves[Random.Range(0, potentialMoves.Count)].Execute();

        GameManager.Instance.EndTurn();
    }

    private void BuildTree(GameNode node, int depth, bool isPlayerTurn)
    {
        if (depth <= 0) return;

        node.GenerateChildren(isPlayerTurn);

        foreach (var child in node.Children.Keys)
        {
            BuildTree(child, depth - 1, !isPlayerTurn);
        }
    }

    private static int Minimax(GameNode node, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || node.IsTerminal) return node.GetHeuristic();

        if (maximizingPlayer)
        {
            var bestValue = int.MinValue;
            foreach (var child in node.Children.Keys)
            {
                var value = Minimax(child, depth - 1, !maximizingPlayer);
                bestValue = Max(bestValue, value);
            }
            node.SetHeuristic(bestValue);
            return bestValue;
        }
        else
        {
            var bestValue = int.MaxValue;
            foreach (var child in node.Children.Keys)
            {
                var value = Minimax(child, depth - 1, !maximizingPlayer);
                bestValue = Min(bestValue, value);
            }
            node.SetHeuristic(bestValue);
            return bestValue;
        }
    }

    private static int Min(int x, int y)
    {
        return x > y ? y : x;
    }

    private static int Max(int x, int y)
    {
        return x < y ? y : x;
    }

    private int[,] ConvertToVirtualBoard(Piece[,] board)
    {
        var virtualBoard = new int[board.GetLength(0), board.GetLength(1)];

        for (var x = 0; x < board.GetLength(0); x++)
        {
            for (var y = 0; y < board.GetLength(1); y++)
            {
                var piece = board[x, y];
                var occupation = BoardOccupation.PLAYER_NONE;

                if (piece == null)
                {
                    virtualBoard[x, y] = occupation;
                    continue;
                }
                
                if (piece.IsOwnedByPlayer && !piece.IsKing) occupation = BoardOccupation.PLAYER_1;
                else if (piece.IsOwnedByPlayer && piece.IsKing) occupation = BoardOccupation.PLAYER_1_KING;
                else if (!piece.IsOwnedByPlayer && !piece.IsKing) occupation = BoardOccupation.PLAYER_2;
                else if (!piece.IsOwnedByPlayer && piece.IsKing) occupation = BoardOccupation.PLAYER_2_KING;

                virtualBoard[x, y] = occupation;
            }
        }

        return virtualBoard;
    }

    private Move CreateMoveFromVirtualMove(VirtualMove virtualMove)
    {
        var board = GameManager.Instance.Board.Pieces;
        var piece = board[virtualMove.Origin.x, virtualMove.Origin.y];
        var dest = new Vector2(virtualMove.Destination.x, virtualMove.Destination.y);

        return virtualMove.IsKill 
            ? new Move(piece, dest, true, board[virtualMove.KillLocation.x, virtualMove.KillLocation.y]) 
            : new Move(piece, dest);
    }

    public void PrintBoard(int[,] state)
    {
        for (var y = state.GetLength(0) - 1; y >= 0; y--)
        {
            var line = "";
            const string space = "  ";

            if (y % 2 != 0) line += space;
            for (var x = 0; x < state.GetLength(1); x++)
            {
                line += state[x, y] + space;
            }
            Debug.Log(line);
        }
    }

    public void PrintBoard(Piece[,] state)
    {
        for (var y = state.GetLength(0) - 1; y >= 0; y--)
        {
            var line = "";
            const string space = "  ";

            if (y % 2 != 0) line += space;
            for (var x = 0; x < state.GetLength(1); x++)
            {
                if (state[x, y] == null)
                {
                    line += "0" + space;
                    continue;
                }
                line += state[x, y] + space;
            }
            Debug.Log(line);
        }
    }
}
