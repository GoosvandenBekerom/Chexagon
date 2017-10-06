using System.Collections.Generic;

public class GameNode
{
    public Piece[,] GameState { get; private set; }
    public List<GameNode> Children { get; private set; }

    public bool IsTerminal
    {
        get { return Children.Count == 0; }
    }

    // Heuristic value
    private bool _calculated = false;
    private int _heuristic;
    public int Heuristic
    {
        get
        {
            if (!_calculated) {
                _heuristic = 1; // TODO: calculate heuristic
            }

            return _heuristic;
        }
    }

    public GameNode(Piece[,] gameState)
    {
        GameState = gameState;
        Children = new List<GameNode>();
    }
}