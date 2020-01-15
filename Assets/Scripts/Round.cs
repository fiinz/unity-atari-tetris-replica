using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round:MonoBehaviour 
{ 
    private int number;
    public int linesObjective;
    public int linesCompleted;
    public int totalTetrominosSpawned;
    public Dictionary<TetroMinoColor, int> tetroMinosSpawned;
    private RoundStats roundStats;
    public GameObject bonusBarPrefab;
    public Player player;

    public delegate void RoundHandler();
    public event RoundHandler GameOverEvent;
    public delegate void RoundCompleteHandler(int row);
    public event RoundCompleteHandler RoundCompleteEvent;

    public void CallRoundCompleteEvent(int row)
    {
        if (RoundCompleteEvent != null)RoundCompleteEvent(row);
    }
    public void CallGameOverEvent()
    {
        if (GameOverEvent != null) GameOverEvent();


    }
    public enum RoundState
    {
        Starting,
        Playing,
        Completed,
        GameOver

    }
    public RoundState state;

    public void Initialize (int number,int linesObjetive, Transform placeStats)
    {
        player = GetComponent<Player>();
        state = RoundState.Starting;
        linesCompleted = 0;
        this.number = number;
        this.linesObjective = linesObjetive;
        tetroMinosSpawned = new Dictionary<TetroMinoColor, int>();
        tetroMinosSpawned.Add(TetroMinoColor.Blue, 0);
        tetroMinosSpawned.Add(TetroMinoColor.Cyan, 0);
        tetroMinosSpawned.Add(TetroMinoColor.Green,0);
        tetroMinosSpawned.Add(TetroMinoColor.Magenta,0);
        tetroMinosSpawned.Add(TetroMinoColor.Orange, 0);
        tetroMinosSpawned.Add(TetroMinoColor.Red, 0);
        tetroMinosSpawned.Add(TetroMinoColor.Yellow,0);
        state = RoundState.Playing;

        if (GameManager.instance.singlePlayer)
        {
            roundStats = GetComponent<RoundStats>();
            roundStats.enabled = true;
            roundStats.Initialize(placeStats);
        }

    }
    

    public void NextRound(int linesObjective)
    {
        foreach(TetroMinoColor type in Enum.GetValues(typeof(TetroMinoColor)))
        {
            tetroMinosSpawned[type] = 0;

        }
        state = RoundState.Starting;
        linesCompleted = 0;
        totalTetrominosSpawned = 0;

    }

    public void ProcessLinesCompleted(int lines)
    {
        linesCompleted += lines;
        if (linesCompleted >= linesObjective)
        {
            ProcessLevelCompleted();
        }
        Debug.Log("current rows" + linesCompleted);


    }
    public void ProcessLevelCompleted()
    {
        state = RoundState.Completed;
        var highestRow=player.currentGrid.GetHighestRowWithMinos();
       StartCoroutine(DrawBonusBars(highestRow));
       // CallRoundCompleteEvent(10);

      //  DrawBonusBars();

    }

     public void IncrementTetroMinosSpawned(TetroMinoColor type)
    {
        totalTetrominosSpawned++;
        tetroMinosSpawned[type]++;
        roundStats.UpdateStats(type,tetroMinosSpawned[type]);
    }


    public IEnumerator DrawBonusBars(int row)
    {
        for (int y = player.currentGrid.height-2; y >row; y--)
        {
            for (int x = 0; x < player.currentGrid.width; x++)
            {
                Debug.Log("DRAW BONUS MINO");
                var minoObjt = MinoPool.instance.GetPooledMinoObject();
                minoObjt.SetActive(true);
                minoObjt.name = "BONUS";
                var spriteIndex=1;
                var newHeadSpriteRenderer = minoObjt.GetComponent<SpriteRenderer>();
                if (x == 0) { spriteIndex = 0; }
                if (x == player.currentGrid.width-1) { spriteIndex = 2; }

                string headSpriteName = "Minos_" + ((int)TetroMinoColor.Gray * 15 + spriteIndex);
                newHeadSpriteRenderer.sprite = GameResources.instance.GetSpriteByName(headSpriteName); //changes the sprite combining the color and the type
                newHeadSpriteRenderer.sortingOrder = 1;

                var childObj = minoObjt.transform.GetChild(0).gameObject;
               // childObj.set


                var spriteRendererChild = minoObjt.transform.GetChild(0).GetComponent<SpriteRenderer>();

                minoObjt.GetComponent<Mino>().enabled = false;
                minoObjt.transform.parent = this.transform;
                minoObjt.transform.localPosition = new Vector2(player.currentGrid.transform.position.x+x, player.currentGrid.transform.position.y + y);
            }
            yield return new WaitForSeconds(0.05f);

        }

    }

    // Start is called before the first frame update


}
