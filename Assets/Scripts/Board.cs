using UnityEngine;

public class Board : MonoBehaviour
{
    public int[,] State { get; set; }

    public void Init()
    {
        var grid = GameManager.Instance.Grid;
        State = new int[grid.GridWidth, grid.GridHeight];
    }
}

public enum TileState
{
    Empty = 0,
    Player1 = 1,
    Player2 = 2
}