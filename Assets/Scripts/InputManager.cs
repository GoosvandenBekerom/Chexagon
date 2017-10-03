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
            var allowedMoves = GameManager.Instance.Board.GetAllowedMoves(selectedPiece);
            GameManager.Instance.Grid.HighlightTiles(allowedMoves);
        }
    }

    void OnTileClick(Transform t)
    {
        if (selectedPiece == null) return;

        var allowed = GameManager.Instance.Grid.HighlightedTiles;
        var tile = t.GetComponent<Tile>();

        if (allowed.ContainsKey(tile))
        {
            selectedPiece.MoveTo(allowed[tile]);
            GameManager.Instance.Grid.UnHighlightTiles();
            selectedPiece = null;
        }
    }
}
