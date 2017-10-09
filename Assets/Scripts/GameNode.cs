using System.Collections.Generic;

public class GameNode
{
    public bool IsPlayerTurn { get; set; }
    public Piece[,] BoardState { get; private set; }
    public List<GameNode> Children { get; private set; }

    public bool IsTerminal
    {
        get { return Children.Count == 0; }
    }

    private bool _calculated;
    private int _heuristic;

    public int Heuristic
    {
        get
        {
            if (!_calculated)
            {
                CalculateHeuristic();
            }
            return _heuristic;
        }
    }

    public GameNode(Piece[,] boardState)
    {
        BoardState = boardState;
        Children = new List<GameNode>();
    }

    private void CalculateHeuristic()
    {
        _heuristic = 1;
        foreach (var piece in BoardState)
        {
            if (piece.IsOwnedByPlayer != IsPlayerTurn) continue;

            bool isKill;
            var moves = BoardMoves.GetAllowedMoves(BoardState, piece, IsPlayerTurn, out isKill);
            
            // TODO: rethink how these gamenodes should be implemented
        }
        _calculated = true;
    }
}