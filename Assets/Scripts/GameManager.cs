using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance ?? new GameManager(); }
    }

    [Header("Script References")]
    public Grid Grid;
    public Board Board;
    public PiecesSpawner PiecesSpawner;
    public AIManager AI;

    [Header("UI References")]
    public Text TurnText;

    public bool IsPlayerTurn { get; private set; }
    public bool PlayingAgainstAI { get; private set; }
    public bool GameOver { get; set; }

    void Awake()
    {
        _instance = this;
        IsPlayerTurn = true;
        PlayingAgainstAI = false;
        TurnText.text = "Player's Turn (White)";
        GameOver = false;
    }

	void Start ()
    {
        Grid.Generate();
        Board.Init();
	    PiecesSpawner.Spawn();
	}

    public void EndTurn()
    {
        if (CheckForWin())
        {
            Debug.Log(IsPlayerTurn ? "Player won" : "AI won");
            EndGame();
            return;
        }

        IsPlayerTurn = !IsPlayerTurn;
        TurnText.text = IsPlayerTurn ? "Player's Turn (White)" : "AI's Turn (Black)";
        Board.UpdateRequiredMoves();

        if (!IsPlayerTurn && PlayingAgainstAI) AI.DoTurn();
    }

    private bool CheckForWin()
    {
        // Check if all pieces are owned by the current player
        var playerHasNoPiecesLeft = Board.Pieces.Cast<Piece>()
            .Where(piece => piece != null)
            .All(piece => piece.IsOwnedByPlayer == IsPlayerTurn);

        if (playerHasNoPiecesLeft) return true;

        // Check if there is an enemy piece with a move left
        foreach (var piece in Board.Pieces)
        {
            if (piece == null || piece.IsOwnedByPlayer == IsPlayerTurn) continue;

            bool isKill;
            if (BoardMoves.GetAllowedMoves(Board.Pieces, piece, !IsPlayerTurn, out isKill).Any())
            {
                return false;
            }
        }

        return true;
    }

    private void EndGame()
    {
        GameOver = true;
        Debug.Log("Game Ended");
    }
}
