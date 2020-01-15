using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject[] pOneLastRowsLeft; //left column text UIS with remaining lines
    public Text[] lines;

    //center column 
    public Text[] playersLinesLeftUI;
    public Text[] playersScores;
    public Text highScore;
    public Text round;
    public Text[] movesScores;

    public void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    public void ShowMovesScore(Player player,int score)
    {

        movesScores[player.id - 1].text = score.ToString();

    }

    public void HideMovesScore(Player player)
    {

        movesScores[player.id - 1].text = "";

    }
    // Start is called before the first frame update
    void Start()
    {

        HideLastRows();
        highScore.text= "0";
        round.text = "1";

    }

    private void UpdatePlayersRowsLeftUI(Player player)
    {

            HideLastRows();       
            if (player.round.state!=Round.RoundState.Completed)
            {
                var rowsLeft = player.round.linesObjective - player.round.linesCompleted;
            
                playersScores[player.id-1].text = rowsLeft.ToString();
                if (rowsLeft <= 5)
                {
                    pOneLastRowsLeft[rowsLeft - 1].SetActive(true);
                }
            }
        
    }
    void UpdateLinesLeftUI(Player player)
    {
        var rowsLeft = player.round.linesObjective - player.round.linesCompleted;
        playersLinesLeftUI[player.id - 1].text = rowsLeft.ToString();
    }

    void UpdatePlayersScoresUI( Player player)
    {
        playersScores[player.id-1].text = (player.stats.score).ToString();
    }

    public void UpdatePlayerRound(Player player)
    {
        round.text = player.currentRound.ToString();
    }

    public void UpdateUIS(Player player)
    {
        UpdatePlayersRowsLeftUI(player);
        UpdatePlayersScoresUI(player);
        UpdateLinesLeftUI(player);
        UpdateLines(player);
        UpdatePlayerRound(player);


    }

    public void UpdateLines(Player player)
    {

        lines[player.id - 1].text = player.round.linesCompleted.ToString();
        
    }


    public void HideLastRows()
    {
        foreach (var line in pOneLastRowsLeft)
        {
            line.SetActive(false);
        }

    }
  

    // Update is called once per frame
    void Update()
    {
        
    }
}
