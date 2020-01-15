using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public sealed class Mino : MonoBehaviour
{
    public string minoSpriteName;
    public Tetromino tetroMinoParent;
    private int[,] tetroMinoMatrix;
    public MinoType minoType;
    public TetroMinoColor tetroMinoType;
    public int row = -1;
    public int col = -1;
    public bool isBeingRemoved = false;
    private bool isInitialized = false;
    public int animateIndex;
    private Animator myAnimator;

    public class MinoParentNotFoundException : Exception
    {
        public MinoParentNotFoundException(){}
        public MinoParentNotFoundException(string message) : base(message){}
        public MinoParentNotFoundException(string message, Exception inner) : base(message, inner){}
    }

    // it saves the line of the matching color in the sprite sheet 0 - 9

    //red =0
    // 


    ///public Vector2Int minoMatrixPos  { get; set; }
    private void Update()
    {
        if (isBeingRemoved)
        {
            AnimateMino();
            myAnimator.enabled = false;
        }
        
        //Debug.Log("parentFolder<color=red>"+transform.parentFolder);
        //if (row == -1 || col == -1 || tetroMinoParent == null)
        //	throw new System.NotImplementedException();
    }


    public void Initialize(int row, int col, TetroMinoColor tetroMinoType, int[,] tetrominoMatrix)
    {
        if (!transform.parent)
        {throw new MinoParentNotFoundException("You need to Set the Parent Before Initializing the Mino");}

        tetroMinoParent = transform.parent.GetComponent<Tetromino>();
        this.tetroMinoMatrix = tetrominoMatrix;
        this.tetroMinoType = tetroMinoType;
        transform.GetChild(0).gameObject.SetActive(false);


        if (row < 0 || col < 0 || row >= tetroMinoMatrix.GetLength(0) || col >= tetroMinoMatrix.GetLength(1))
            throw new IndexOutOfRangeException();
        
        
            this.row = row;
            this.col = col;
            DrawMino();


        if (!isInitialized) isInitialized = true;
        isBeingRemoved = false;
        myAnimator = GetComponent<Animator>();
         myAnimator.enabled = false;

        if (tetroMinoType == TetroMinoColor.Gray)
        {
            myAnimator.enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);

        }
    }

    public void RemoveMinoObject()
    {
        //   MinoPool.instance.DisableMino(this)
        //gameObject.transform.name = "SUPOSELY_REMOVED_FROM_GRID";
        if (tetroMinoType == TetroMinoColor.Gray)
        {
            myAnimator.enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        transform.SetParent(MinoPool.instance.poolParent.transform);
        transform.position = Vector2.zero;
    }

    public void AnimateMino()
    {
        //randomizes 7
        
        minoSpriteName = "Minos_" + ((int)TetroMinoColor.MultiColor * 15 + animateIndex);
        GetComponent<SpriteRenderer>().sprite = GameResources.instance.GetSpriteByName(minoSpriteName); //changes the sprite combining the color and the type
        animateIndex++;
        if (animateIndex > 5) animateIndex = 0;

    }

    public void UpdateMino()
    {
        DrawMino();
    }

    public void ReDrawMino()
    {
        minoType = GetMinoType(row, col);
        minoSpriteName = "Minos_" + ((int) tetroMinoType * 15 + (int) minoType);
        GetComponent<SpriteRenderer>().sprite =GameResources.instance.GetSpriteByName(minoSpriteName); //changes the sprite combining the color and the type
    }

    private void DrawMino()
    {
        //	if(row==-1 || col == -1) { throw new System.NotImplementedException();}
        GetComponent<SpriteRenderer>().sortingOrder = 1; // sets the order to the max
        minoType = GetMinoType(row, col);
        minoSpriteName = "Minos_" + ((int) tetroMinoType * 15 + (int) minoType);
      //  Debug.Log("mino sprite" + minoSpriteName);
        GetComponent<SpriteRenderer>().sprite =GameResources.instance.GetSpriteByName(minoSpriteName); //changes the sprite combining the color and the type
    }


    private int[] CheckNeighbours(int row, int col)
    {
        //check neighbour values
        int[] neighbours = new int[4];
        //0 left // 1 top // 2 right // 3 bottom

        if (row < tetroMinoMatrix.GetLength(0) - 1)
            neighbours[1] = tetroMinoMatrix[row + 1, col];

        if (row > 0)
            neighbours[3] = tetroMinoMatrix[row - 1, col];

        if (col < tetroMinoMatrix.GetLength(1) - 1)
            neighbours[2] = tetroMinoMatrix[row, col + 1];

        if (col > 0)
            neighbours[0] = tetroMinoMatrix[row, col - 1];


        return neighbours;
    }

    private MinoType GetMinoType(int row, int col)
    {
        int[] neighBours = CheckNeighbours(row, col);
        int totalNeighbours = neighBours[0] + neighBours[1] + neighBours[2] + neighBours[3];
        //process neighboors
        switch (totalNeighbours)
        {
            case 0: return MinoType.Single;
            case 1:

                if (neighBours[0] == 1) //se em baixo ha uma peça quer dizer que esta tem de ser a de  right
                    return MinoType.Right;
                if (neighBours[2] == 1) //se em baixo ha uma peça quer dizer que esta tem de ser a de left
                    return MinoType.Left;
                if (neighBours[1] == 1) //se em cima ha uma peça entao esta tem de ser a bottom
                    return MinoType.Bottom;
                if (neighBours[3] == 1) //se em baixo ha uma peça quer dizer que esta tem de ser a de topo
                    return MinoType.Top;

                break;

            //0 left // 1 top // 2 right // 3 bottom


            case 2:
                if (neighBours[0] + neighBours[3] == 2)
                    return MinoType.Corner_Bottom_Left;

                if (neighBours[0] + neighBours[1] == 2)
                    return MinoType.Corner_Top_Left;

                if (neighBours[2] + neighBours[3] == 2)
                    return MinoType.Corner_Bottom_Right;

                if (neighBours[2] + neighBours[1] == 2)
                    return MinoType.Corner_Top_Right;

                if (neighBours[0] + neighBours[2] == 2)
                    return MinoType.TwoSidedHorizontal;

                if (neighBours[1] + neighBours[3] == 2)
                    return MinoType.TwoSidedVertical;

                break;


            case 3:

                //0 left // 1 top // 2 right // 3 bottom
                if (neighBours[1] + neighBours[2] + neighBours[3] == 3)
                    return MinoType.TRight;

                if (neighBours[0] + neighBours[2] + neighBours[3] == 3)
                    return MinoType.TBottom;

                if (neighBours[0] + neighBours[1] + neighBours[2] == 3)
                    return MinoType.TTop;

                if (neighBours[0] + neighBours[1] + neighBours[3] == 3)
                    return MinoType.TLeft;
                break;
        }

        return MinoType.Single;
    }
}