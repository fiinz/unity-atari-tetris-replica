using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour
{

	public int id = -1; //the pfPlayer id
	public bool isInitialized = false;
	public PlayerInput input;
    public Round round;
    public RoundStats roundStats;
    public PlayerStats stats;
    public int currentRound; //passar isto para dentro do round       
    public PlayerGrid currentGrid;

    public void Initialize(int id, Transform placeStats)
	{
		this.id = id;
        stats = new PlayerStats(this);
		isInitialized = true;
		input = GetComponent<PlayerInput>();
        round=GetComponent<Round>();
        currentRound = 1;
        round.Initialize(currentRound,currentRound + 5, placeStats);
      
         UIManager.instance.UpdatePlayerRound(this);
	}
    

	 void Update()
	{
       
        //testing tetromino insertion 
        /*if (Input.GetKeyDown(KeyCode.A))
		{

			Vector2 randomPos = new Vector2(UnityEngine.Random.Range(0, playerGrid.width),
				UnityEngine.Random.Range(0, playerGrid.height)); 

			Debug.Log(playerGrid.GetTransformAtGridPosition(randomPos));


			InsertTetrominoAtPos(randomPos);

		}*/
    }




}
