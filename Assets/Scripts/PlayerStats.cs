using System.Collections;
using System.Collections.Generic;


public class PlayerStats 
{

    private Player player;
    public int score;
   

    public PlayerStats(Player player)
    {
        this.player = player;
        score = 0;
         
    }

    public void UpdatePlayerScore(int addScore)
    {

        score += addScore;

    }


}
