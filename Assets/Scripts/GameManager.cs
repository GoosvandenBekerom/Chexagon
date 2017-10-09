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
    public GameObject MenuPanel;
    public GameObject WinPanel;
    public Text WinTitle;

    public bool IsPlayerTurn { get; private set; }
    public bool PlayingAgainstAI { get; private set; }
    public bool GameOver { get; set; }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        MenuPanel.SetActive(true);
        WinPanel.SetActive(false);

        Grid.Generate();
        Board.Init();
    }

	public void StartGame (bool againstAI)
    {
        PlayingAgainstAI = againstAI;
        IsPlayerTurn = true;
        TurnText.text = "Player's Turn (White)";
        GameOver = false;

        Board.ClearBoard();
	    PiecesSpawner.Spawn();
        MenuPanel.SetActive(false);
	}

    public void ReturnToMainMenu()
    {
        MenuPanel.SetActive(true);
        WinPanel.SetActive(false);
    }

    public void EndTurn()
    {
        var player2text = PlayingAgainstAI ? "AI" : "Player 2";

        if (CheckForWin())
        {
            EndGame(IsPlayerTurn ? "Player won" : player2text + " won");
            return;
        }

        IsPlayerTurn = !IsPlayerTurn;
        TurnText.text = IsPlayerTurn ? "Player's Turn (White)" : player2text + "'s Turn (Black)";
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

    private void EndGame(string winText)
    {
        GameOver = true;
        Board.ClearBoard();
        WinTitle.text = winText;
        WinPanel.SetActive(true);
    }
}
