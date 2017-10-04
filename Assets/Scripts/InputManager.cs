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

        if (!piece.IsOwnedByPlayer) return;

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
            
            GameManager.Instance.Grid.HighlightTiles(allowedMoves, isKill);
        }
    }

    void OnTileClick(Transform t)
    {
        if (selectedPiece == null) return;

        var moves = GameManager.Instance.Grid.HighlightedTiles;
        var tile = t.GetComponent<Tile>();

        if (moves.ContainsKey(tile))
        {
            moves[tile].Execute();
            GameManager.Instance.Grid.UnHighlightTiles();
            selectedPiece = null;
        }
    }
}
