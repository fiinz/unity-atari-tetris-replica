using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Player Input of the Player
/// </summary>
public class PlayerInput : MonoBehaviour
{

	public int xAxis; //checks x ( -1, 0 , 1 ) input
	public int yAxis; // checks down input


	public bool rotate; //checks rotation button
	public bool pressing;
	

    public void ResetAxis()
	{
		if (!pressing) //resets axis if player is not pressing
			xAxis = 0; //reseting the axis
	 //reseting the axis
		rotate = false;
	}

	// Update is called once per frame
	void Update()
	{

		
    	if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			xAxis = -1; //left small left
			pressing = true;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			xAxis = 1; //left small left
			pressing = true;

		}
		

		if (Input.GetKeyUp(KeyCode.LeftArrow))  //Get Key So The Player Can move The Tetromino without Releasing the key
		{
			//	ResetAxis();
			pressing = false;
		}

		if (Input.GetKeyUp(KeyCode.RightArrow)) //Get Key So The Player Can move The Tetromino without Releasing the key
		{
			pressing = false;
			//ResetAxis();
		}


		//rotate
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
		
			rotate = true;
		}

		//drop 
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			yAxis = -1;
		}

		if (Input.GetKeyUp(KeyCode.DownArrow)) //Get Key So The Player Can move The Tetromino without Releasing the key
		{
			yAxis = 0;
		}

		
		

		//pressed time = how much time has been passed since the user pressed key

		//ResetAxis();
	}



}
