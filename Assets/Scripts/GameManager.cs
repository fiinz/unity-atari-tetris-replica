
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int highscore;
	public static GameManager instance;
	public int numOfPlayers; //set on inspector
	public GameObject pfPlayer; //pfPlayer prefab
	public GameObject pfGrid;
	public List<Player> players;
	public Transform playerOneNextTetrominoPlaceHolder;
	public Transform playerTwoNextTetromino;
	public Transform playerOneGrid;
    private Vector2 playerOneGridRelativeSpawnPoint;
	public Transform playerTwoGrid;
    private Vector2 playerTwoGridRelativeSpawnPoint;
    private bool addedPlayers = false;
    public AudioClip[] musicClips;
    public bool singlePlayer=true;

    public enum GameDifficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2

    }
    public int TopLimit = 20;

    public GameDifficulty gameDifficulty;

    private void Awake()
	{
		
		if (instance == null) instance = this;
		else{Destroy(this);}

    }
    void Start()
    {
        Sound2DManager.instance.PlayMusic(musicClips[0]);
        AddPlayers();
        gameDifficulty = GameDifficulty.Hard;
    }


    
    private void Update()
    {

        UIManager.instance.UpdateUIS(players[0]);

    }
    public void AddPlayers()
     
	{
		//ignoring num of players for now
		GameObject newPlayerObject= Instantiate(pfPlayer, new Vector2(0, 0), Quaternion.identity);
		GameObject gridObject= Instantiate(pfGrid, playerOneGrid.position, Quaternion.identity);
        playerOneGridRelativeSpawnPoint=new Vector2(4, 21);

        if (!addedPlayers)
        {
            Player player = newPlayerObject.GetComponent<Player>();
            PlayerGrid playerGrid = gridObject.GetComponent<PlayerGrid>();
            player.Initialize(1,playerTwoGrid);
            playerGrid.Initialize(player, playerOneNextTetrominoPlaceHolder.position, playerOneGridRelativeSpawnPoint, 10, 21);
            players.Add(player);

        }

    }


	public void GameOver()
	{
		SceneManager.LoadScene("GameOver");
	}

}
