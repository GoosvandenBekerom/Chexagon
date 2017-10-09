using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public void DoTurn()
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

    private int Minimax(GameNode node, int depth)
    {
        if (depth == 0 || node.IsTerminal) return node.Heuristic;

        if (!node.IsPlayerTurn)
        {
            var bestValue = 0;
            foreach (var child in node.Children)
            {
                var value = Minimax(child, depth - 1);
                bestValue = Max(bestValue, value);
            }
            return bestValue;
        }
        else
        {
            var bestValue = int.MaxValue;
            foreach (var child in node.Children)
            {
                var value = Minimax(child, depth - 1);
                bestValue = Min(bestValue, value);
            }
            return bestValue;
        }
    }

    /// <summary>
    /// Linq variant of minimax algorithm
    /// TODO: Test if this is more or less efficient
    /// </summary>
    /// <param name="node"></param>
    /// <param name="depth"></param>
    /// <param name="maximizingPlayer"></param>
    /// <returns></returns>
    private int MinimaxLinq(GameNode node, int depth)
    {
        if (depth == 0 || node.IsTerminal) return node.Heuristic;

        return !node.IsPlayerTurn
            ? node.Children.Select(child => MinimaxLinq(child, depth - 1)).Aggregate(0, Max)
            : node.Children.Select(child => MinimaxLinq(child, depth - 1)).Aggregate(int.MaxValue, Min);
    }

    private static int Min(int x, int y)
    {
        return x > y ? y : x;
    }

    private static int Max(int x, int y)
    {
        return x < y ? y : x;
    }
}
