using UnityEngine;

public class Board : MonoBehaviour
{
    public Piece[,] Pieces { get; set; }

    public void Init()
    {
        var grid = GameManager.Instance.Grid;
        Pieces = new Piece[grid.GridWidth, grid.GridHeight];
    }
}