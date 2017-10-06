using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{


    private int Minimax(GameNode node, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || node.IsTerminal) return node.Heuristic;

        if (maximizingPlayer)
        {
            var bestValue = 0;
            foreach (var child in node.Children)
            {
                var value = Minimax(child, depth - 1, false);
                bestValue = Max(bestValue, value);
            }
            return bestValue;
        }
        else
        {
            var bestValue = int.MaxValue;
            foreach (var child in node.Children)
            {
                var value = Minimax(child, depth - 1, true);
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
    private int MinimaxLinq(GameNode node, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || node.IsTerminal) return node.Heuristic;

        return maximizingPlayer
            ? node.Children.Select(child => MinimaxLinq(child, depth - 1, false)).Aggregate(0, Max)
            : node.Children.Select(child => MinimaxLinq(child, depth - 1, true)).Aggregate(int.MaxValue, Min);
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
