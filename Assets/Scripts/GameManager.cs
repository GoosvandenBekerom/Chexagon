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

    [Header("UI References")]
    public Text TurnText;

    public bool IsPlayerTurn { get; private set; }

    void Awake()
    {
        _instance = this;
        IsPlayerTurn = true;
        TurnText.text = "Player's Turn (White)";
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

        if (CheckForDoubleJump())
        {
            // TODO: start second jump "turn"
            return;
        }

        IsPlayerTurn = !IsPlayerTurn;
        TurnText.text = IsPlayerTurn ? "Player's Turn (White)" : "AI's Turn (Black)";
        Board.UpdateRequiredMoves();
    }

    private bool CheckForWin()
    {
        // Check if all pieces are owned by the current player
        return Board.Pieces.Cast<Piece>()
            .Where(piece => piece != null)
            .All(piece => piece.IsOwnedByPlayer == IsPlayerTurn);
    }

    private bool CheckForDoubleJump()
    {
        Debug.Log("Check for double jump not implemented");
        return false; // TODO: implement this check
    }

    private void EndGame()
    {
        Debug.Log("Game Ended");
    }
}
