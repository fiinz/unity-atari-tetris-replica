using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundStats:MonoBehaviour 
{
    public Transform[,] statsMatrix;
    private Transform place;

    public void Initialize (Transform place)
    {
        this.place = place;
        statsMatrix = new Transform[22,7];
        // round = GetComponent<Round>();
    }

    public void ClearStats()
    {
        for (int y = 0; y < statsMatrix.GetLength(0); y++)
        {
            for (int x = 0; y < statsMatrix.GetLength(1); x++)
            {
                statsMatrix[y, x].gameObject.SetActive(false);
                statsMatrix[y, x].SetParent(MinoPool.instance.poolParent.transform);
                statsMatrix[y, x].position = Vector2.zero;
            }
        }
               
    }
   

    public void UpdateStats(TetroMinoColor type,int number)
    {
        Debug.Log("New Entry Stat");
//        int x = 28;
        if (number > 1) { 
        var previousHeadSpriteRenderer= statsMatrix[number - 2, (int)type].GetComponent<SpriteRenderer>();
        string previousSpriteName = "Minos_" + ((int)type * 15 + 4);
        previousHeadSpriteRenderer.sprite = GameResources.instance.GetSpriteByName(previousSpriteName); //changes the sprite combining the color and the type;

        }
        
        var newHead =MinoPool.instance.GetPooledMinoObject();
        newHead.GetComponent<Mino>().enabled = false;
        newHead.SetActive(true);
        newHead.transform.parent = this.transform;
        var newHeadSpriteRenderer=newHead.GetComponent<SpriteRenderer>();
        statsMatrix[number - 1, (int)type] = newHead.transform;
        newHead.transform.parent = place;
        newHead.transform.localPosition = new Vector2(1+(int)type,number-1);
       
        string headSpriteName = "Minos_" + ((int)type * 15 + 3);
        newHeadSpriteRenderer.sprite = GameResources.instance.GetSpriteByName(headSpriteName); //changes the sprite combining the color and the type
        newHeadSpriteRenderer.sortingOrder = 1;
    }


}

    // Start is called before the first frame update


