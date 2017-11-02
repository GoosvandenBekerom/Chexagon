using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class GameNode
{
    public int[,] BoardState { get; private set; }
    public GameNode Parent { get; private set; }
    public bool HasParent { get { return Parent != null; } }
    public Dictionary<GameNode, VirtualMove> Children { get; private set; }
    public bool IsTerminal { get { return Children.Count == 0; } }
    public bool IsKillingState { get; private set; }
    
    public GameNode(int[,] boardState, GameNode parent = null)
    {
        BoardState = boardState;
        Parent = parent;
        Children = new Dictionary<GameNode, VirtualMove>();
        IsKillingState = false;
    }
    
    public void GenerateChildren(bool isPlayerTurn)
    {
        var moves = new List<VirtualMove>();
        for (var x = 0; x < BoardState.GetLength(0); x++)
        {
            for (var y = 0; y < BoardState.GetLength(1); y++)
            {
                var current = BoardState[x, y];
                if (current == BoardOccupation.PLAYER_NONE) continue;

                var player = isPlayerTurn ? BoardOccupation.PLAYER_1 : BoardOccupation.PLAYER_2;
                var playerKing = isPlayerTurn ? BoardOccupation.PLAYER_1_KING : BoardOccupation.PLAYER_2_KING;

                if (current != player && current != playerKing) continue;

                bool isKill;
                var possibleMoves = VirtualBoardMoves.GetAllowedMoves(BoardState, new VirtualTile(x, y), isPlayerTurn, out isKill);
                
                if (!isKill && !IsKillingState)
                {
                    moves.AddRange(possibleMoves);
                }
                else if (isKill && !IsKillingState)
                {
                    moves.Clear();
                    moves.AddRange(possibleMoves);
                    IsKillingState = true;
                }
                else if (isKill && IsKillingState)
                {
                    moves.AddRange(possibleMoves);
                }
            }
        }

        foreach (var move in moves)
        {
            // Copy the current board
            var newState = (int[,])BoardState.Clone();
            // Execute the virtual move that this node should represent
            newState[move.Destination.x, move.Destination.y] = newState[move.Origin.x, move.Origin.y];
            newState[move.Origin.x, move.Origin.y] = BoardOccupation.PLAYER_NONE;
            if (move.IsKill) newState[move.KillLocation.x, move.KillLocation.y] = BoardOccupation.PLAYER_NONE;

            Children.Add(new GameNode(newState, this), move);
        }
    }

    private bool _calculated;
    private int _heuristic;

    /// <summary>
    /// Player 1 wants the highest possible heuristic score, Player 2 wants the lowest possible heuristic score
    /// </summary>
    public int GetHeuristic()
    {
        if (_calculated) return _heuristic;

        _heuristic = 0;
        
        // first check if this board is a win
        var player1Win = true;
        var player2Win = true;
        foreach (var spot in BoardState)
        {
            switch (spot)
            {
                case BoardOccupation.PLAYER_NONE:
                    continue;
                case BoardOccupation.PLAYER_2:
                case BoardOccupation.PLAYER_2_KING:
                    player1Win = false;
                    break;
                case BoardOccupation.PLAYER_1:
                case BoardOccupation.PLAYER_1_KING:
                    player2Win = false;
                    break;
                default:
                    continue;
            }

            if (!player1Win && !player2Win) break;
        }

        if (player1Win)
        {
            _heuristic = BoardStateScores.WIN;
            _calculated = true;
            return _heuristic;
        }
        if (player2Win)
        {
            _heuristic = -BoardStateScores.WIN;
            _calculated = true;
            return _heuristic;
        }

        // If the game is not over, score this board on its state
        for (var x = 0; x < BoardState.GetLength(0); x++)
        {
            for (var y = 0; y < BoardState.GetLength(1); y++)
            {
                var owner = BoardState[x, y];
                if (owner == BoardOccupation.PLAYER_NONE) continue;

                // Score for pieces on the board
                if (owner == BoardOccupation.PLAYER_1) _heuristic += BoardStateScores.PIECE_NORMAL;
                else if (owner == BoardOccupation.PLAYER_1_KING) _heuristic += BoardStateScores.PIECE_KING;
                else if (owner == BoardOccupation.PLAYER_2) _heuristic -= BoardStateScores.PIECE_NORMAL;
                else if (owner == BoardOccupation.PLAYER_2_KING) _heuristic -= BoardStateScores.PIECE_KING;
            }
        }
        
        _calculated = true;
        return _heuristic;
    }

    public void SetHeuristic(int heuristic)
    {
        _calculated = true;
        _heuristic = heuristic;

        if (HasParent)
        {
            Parent.SetHeuristic(_heuristic);
        }
    }
}