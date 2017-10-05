using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Piece selectedPiece;

    void Start()
    {
        selectedPiece = null;
    }

	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
	        {
	            switch (hit.transform.tag)
	            {
                    case "Piece":
	                    OnPieceClick(hit.transform);
	                    break;
                    case "Tile":
	                    OnTileClick(hit.transform);
	                    break;
	            }
	        }
        }
	}

    void OnPieceClick(Transform t)
    {
        var piece = t.GetComponent<Piece>();

        //if (!piece.IsOwnedByPlayer) return; //TODO: replace this when AI is implemented
        if (piece.IsOwnedByPlayer != GameManager.Instance.IsPlayerTurn) return;

        var requiredToMove = GameManager.Instance.Board.RequiredMoves;
        if (requiredToMove.Count > 0 && requiredToMove.All(m => m.Piece != piece))
        {
            GameManager.Instance.Grid.HighlightRequiredTiles(requiredToMove);
            return;
        }
        
        if (piece == selectedPiece)
        {
            selectedPiece = null;
            piece.UnHighlight();
            GameManager.Instance.Grid.UnHighlightTiles();
        }
        else
        {
            if (selectedPiece != null)
            {
                selectedPiece.UnHighlight();
                GameManager.Instance.Grid.UnHighlightTiles();
            }

            selectedPiece = piece;
            selectedPiece.Highlight();
            bool isKill;
            var allowedMoves = GameManager.Instance.Board.GetAllowedMoves(selectedPiece, out isKill);
            
            GameManager.Instance.Grid.HighlightMoves(allowedMoves, isKill);
        }
    }

    void OnTileClick(Transform t)
    {
        if (selectedPiece == null) return;

        var moves = GameManager.Instance.Grid.HighlightedTiles;
        var tile = t.GetComponent<Tile>();

        if (moves.ContainsKey(tile))
        {
            var move = moves[tile];
            moves[tile].Execute();
            
            GameManager.Instance.Grid.UnHighlightTiles();
            GameManager.Instance.Grid.UnHighlightRequiredTiles();

            if (move.IsKill)
            {
                // Check if its possible to do a double jump
                if (GameManager.Instance.Board.UpdateRequiredMoves(selectedPiece))
                {
                    GameManager.Instance.Grid.HighlightMoves(GameManager.Instance.Board.RequiredMoves, true);
                    return;
                }
            }

            selectedPiece = null;
            GameManager.Instance.EndTurn();
        }
    }
}
