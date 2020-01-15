using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


//responsible for the tetromino movement
public class TetrominoMovement : MonoBehaviour
{

    private float moveDownCurrentRate; // current drop rate
    private float moveDownNormalRate = 0.45f; // the normal drop timer
    private float moveDownMinRate = 0.0005f; // the mininum timer for a tetromino to drop
    private float moveDownDeductionStep = 0.25f; // ammount of time reduced each iteration

    private float nextMoveDownTime;//next time the Tetromino the player will drop the piece

    private float nextHorizontalMove;// when will next horizontal move from left to right
    private float nextHorizontalMoveTime = 0.125f; //the rate of moving the tetromino

    private float nextAutomaticFall; //when will the tetromino automatic fall 
    private float automaticFallRate;
  

    private Tetromino tetromino; // the tetrominoreference.
    private PlayerInput playerInput; // to wich input the tetromino 
    private PlayerGrid grid; // the grid where the tetromino will be inserted
    public bool colided = false; // if the tetromino colided to the y limits or other piece
    public bool isInitialized = false; // if the TetrominoMovement is initialized from outside

    public void Initialize(PlayerGrid playerGrid, Tetromino tetromino)
    {
        isInitialized = true;
        enabled = true;
        this.colided = false;
        this.grid = playerGrid;
        this.tetromino = tetromino;
        this.playerInput = playerGrid.player.input;
        automaticFallRate = 0.45f - (playerGrid.player.currentRound*0.05f);
        if (automaticFallRate < 0.05f) automaticFallRate = 0.05f;
        nextAutomaticFall = Time.time + automaticFallRate;
        nextHorizontalMove = Time.time + nextHorizontalMoveTime;
        nextMoveDownTime = Time.time + moveDownCurrentRate;
        ResetDropRate();
    }




    public void DisableMovement()
    {

        //Debug.Log("Disabled Movement");
        this.enabled = false;
        //Destroy(this);

    }

    /// <summary>
    /// Fall is the automatic y movement of the tetromino
    /// </summary>
    /// <returns></returns>
    private bool ProcessFall()
    {
        if (colided) return true;

        if (Time.time > nextAutomaticFall)
        {

            nextAutomaticFall = Time.time + automaticFallRate;
            //needs to be changed
            if (grid.NextMoveIsPossible(tetromino, new Vector3(0, -1, 0))) // check if the movement in the grid its possible
            {
                //it translate all the tetromino
                transform.Translate(Vector2.down);
                //updates the matrix
                grid.UpdateTetrominoInGrid(tetromino);
                return true;

            }
            else
            {
                if (!colided) //avoids calling checkrows 2 times // the movement is not possible
                {
                    colided = true;
                    grid.CheckRowsForClear();
                    DisableMovement();

                }
                return true;

            }
        }

        return false;
    }

    /// <summary>
    /// resets the y increment for dropping the tetromino while the user is holding down key
    /// </summary>
    private void ResetDropRate()
    {
        moveDownCurrentRate = moveDownNormalRate;
    }


    /// <summary>
    /// Horizontal move of the tetromino
    /// </summary>
    /// <returns></returns>
    private bool ProcessHorizontalMove()
    {
        if (colided) return true;

        if (!playerInput.pressing)
        {
            //if the player is not pressing then resets the move timer
            nextHorizontalMove = Time.time;
        }

        if (Time.time > nextHorizontalMove) // nextmove is on horizontal  
        {

            nextHorizontalMove = Time.time + nextHorizontalMoveTime;
            //1 its a keypressed move
            if (playerInput.xAxis != 0)
            {

                //needs to be changed
                if (grid.NextMoveIsPossible(tetromino, new Vector2(playerInput.xAxis, 0)))
                {
                    //it translate all the tetromino
                    transform.Translate(new Vector2(playerInput.xAxis, 0));
                    grid.UpdateTetrominoInGrid(tetromino);
                    playerInput.ResetAxis();
                    return true;

                }
            }

        }
        return false;
    }



    /// <summary>
    /// When the user keeps the down button pressed it drops faster and faster
    /// </summary>
    void ReduceDropRate()
    {
        
        moveDownCurrentRate -= (moveDownDeductionStep);
        if (moveDownCurrentRate < moveDownMinRate) moveDownCurrentRate = moveDownMinRate;
    }

    bool ProcessDrop()
    {

        if (colided) return true;
        if (Time.time > nextMoveDownTime)
        {
            ReduceDropRate();
            nextMoveDownTime = Time.time + moveDownCurrentRate;

            if (playerInput.yAxis != 0)
            {
                if (grid.NextMoveIsPossible(tetromino, new Vector2(0, -1)))
                {
                    //it translate all the tetromino
                    transform.Translate(Vector2.down);
                    grid.UpdateTetrominoInGrid(tetromino);
                    return true;
                }
                else
                {
                    //é obrigatório nos movimentos verticais logo que não der para descer remover o componente
                    //caso contrário o fall será ativado e volta  adescer.
                    ResetDropRate();
                    grid.CheckRowsForClear();
                    DisableMovement();
                    playerInput.yAxis=0;
                    return true;

                }
            }
            else
            {
                
                nextMoveDownTime = Time.time;
                ResetDropRate();
                return true;
            }
        }

        return false;
    }

    bool ProcessRotate()
    {

        if (playerInput.rotate == true)
        {
            //int[,] tempTetrominoMatrix = (int[,])tetromino.tetrominoMatrix.Clone();
            if (grid.CheckIfNextRotationIsPossible(tetromino))
            {
                //it translate all the tetromino
                tetromino.RotateTetromino();
                grid.UpdateTetrominoInGrid(tetromino);
                playerInput.ResetAxis();
                return true;
            }
        }

        return false;
    }

    //
    
    void Update()
    {
        if (!isInitialized)
        {  //Debug.LogError("tetromino movement not initialized"); 
            return;
        }


        if (isInitialized && tetromino != grid.currentTetromino)
        {
            //Debug.LogError("tetromino is not the current ");
            return;
        }
        if (playerInput == null) return;
        if (grid.currentTetromino != tetromino)
        { this.gameObject.SetActive(false); return; }
        
        ProcessFall();
        ProcessHorizontalMove();
        //ProcessLongHorizontalMove();
        ProcessRotate();
        ProcessDrop();


       
    }

}
