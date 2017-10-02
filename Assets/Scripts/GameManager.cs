using UnityEngine;

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

    void Awake()
    {
        _instance = this;
    }

	void Start ()
    {
        Grid.Generate();
        Board.Init();
	    PiecesSpawner.Spawn();
	}
}
