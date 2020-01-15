using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tetromino : MonoBehaviour
{

    public TetroMinoColor type;
    public int[,] tetrominoMatrix;
    private GameObject pFMino;
    public List<Mino> minos = new List<Mino>();

   
    private int currentRotation = 0;
    public int currentX;
    public int currentY;
    private PlayerGrid _playerGrid;
    public bool isInitialized = false;
    public bool disabling = false;

    private int[,] GRAY_SINGLE = {
        {1}
    };

    //u gotta imagined rotated Y X
    private int[,] BLUE_SQUARE = 
    {
        {1, 1,},
        {1, 1,},

    };

    // y . x   
    private int[,] GREEN_T =  //the definition is y x 
	{
        {0,1,0,},
        {1,1,1,},
        {0,0,0,},

    };

    private int[,] RED_I = //the definition is y x 
	{

        {0,0,0,0},
        {1,1,1,1},
        {0,0,0,0},
        {0,0,0,0}

    };

    private int[,] ORANGE_S = //the definition is y x 
	{
        {1,1,0},
        {0,1,1},
        {0,0,0},

    };
    private int[,] CYAN_S = //the definition is y x 
	{
        {0,1,1},
        {1,1,0},
        {0,0,0}
    };


    private int[,] YELLOW_J =//the definition is y x 
	{
        {1,1,1},
        {0,0,1},
        {0,0,0}
     };

    private int[,] MAGENTA_L = //the definition is y x 
	{
        {1,1,1},
        {1,0,0},
        {0,0,0},

    };


   /// <summary>
    /// inits a tetromino with a specific color
    /// </summary>
    /// <param name="type"></param>
    public void Initialize(PlayerGrid playerGrid, TetroMinoColor type)
    {
        disabling = false;
        minos.Clear();
        gameObject.SetActive(true);
        isInitialized = true;
        this._playerGrid = playerGrid;
        this.type = type;
        SetTetroMinoMatrix(type);
        if (tetrominoMatrix != null)DrawTetroMino();

    }
    private void Update()
    {
        if (!disabling)
        {
            CheckTetrominoDisabling();
        }
        
    }

    public void CheckTetrominoDisabling()
    {
       // Debug.Log(transform.name + "_CHECKING CHILDS_" + transform.childCount);
        if (this.transform.childCount == 0)
        {
            disabling = true;
           StartCoroutine(ProcessTetrominoDisabling());

        }
    }
    public void ClearTetromino()
    {
        if (minos.Count > 0)
        {
            for(int i = minos.Count - 1; i >= 0; i--)
            {
                RemoveMinoFromMatrix(minos[i]);

            }

        }
       
        tetrominoMatrix = null;
        minos.Clear();
    }

    private IEnumerator ProcessTetrominoDisabling()
    {
        yield return new WaitForSeconds(0.5f);
       // Debug.LogError("disabling mino");
        tetrominoMatrix = null;
        gameObject.SetActive(false);
        transform.SetParent(TetrominoPool.instance.tetrominoParent.transform);
        transform.position = Vector2.zero;


    }


    private void SetTetroMinoMatrix(TetroMinoColor type) // sets the array acordingly to the color
    {

        switch (type)
        {

            case TetroMinoColor.Gray:

                tetrominoMatrix = new int[GRAY_SINGLE.GetLength(0), GRAY_SINGLE.GetLength(1)];
                tetrominoMatrix = (int[,])GRAY_SINGLE.Clone();

                break;

            case TetroMinoColor.Blue:
                tetrominoMatrix = new int[BLUE_SQUARE.GetLength(0), BLUE_SQUARE.GetLength(1)];
                tetrominoMatrix = (int[,])BLUE_SQUARE.Clone();
                break;
            case TetroMinoColor.Cyan:

                tetrominoMatrix = new int[CYAN_S.GetLength(0), CYAN_S.GetLength(1)];
                tetrominoMatrix = (int[,])CYAN_S.Clone();
                break;
            case TetroMinoColor.Red:
                tetrominoMatrix = new int[RED_I.GetLength(0), RED_I.GetLength(1)];
                tetrominoMatrix = (int[,])RED_I.Clone();
                break;
            case TetroMinoColor.Magenta:

                tetrominoMatrix = new int[MAGENTA_L.GetLength(0), MAGENTA_L.GetLength(1)];
                tetrominoMatrix = (int[,])MAGENTA_L.Clone();
                break;
            case TetroMinoColor.Yellow:

                tetrominoMatrix = new int[YELLOW_J.GetLength(0), YELLOW_J.GetLength(1)];
                tetrominoMatrix = (int[,])YELLOW_J.Clone();
                break;
            case TetroMinoColor.Green:
                tetrominoMatrix = new int[GREEN_T.GetLength(0), GREEN_T.GetLength(1)];
                tetrominoMatrix = (int[,])GREEN_T.Clone();
                break;
            case TetroMinoColor.Orange:

                tetrominoMatrix = new int[ORANGE_S.GetLength(0), ORANGE_S.GetLength(1)];
                tetrominoMatrix = (int[,])ORANGE_S.Clone();
                break;

        }
    }

    // Function for do 
    // transpose of matrix  //a transposta troca linhas de uma matriz por colunas
    void Transpose(int[,] currentTetrominoMatrix)
    {
        int numRows = currentTetrominoMatrix.GetLength(0);
        int numCols = currentTetrominoMatrix.GetLength(1);

        //a diagonal da transposta nao altera 
        for (int row = 0; row < numRows; row++)
        { 
            for (int col = row; col < numCols; col++)
            {
                int temp = currentTetrominoMatrix[row, col];
                currentTetrominoMatrix[row, col] = currentTetrominoMatrix[col, row];
                currentTetrominoMatrix[col, row] = temp;
            }
        }
    }

    public void RotateTetrominoMatrix(bool inverse)
    {
        if (!inverse)
        {
            Transpose(tetrominoMatrix);
            ReverseColumns(tetrominoMatrix);
            currentRotation += 90;
        }
        else
        {
            ReverseColumns(tetrominoMatrix);
            Transpose(tetrominoMatrix);
            currentRotation = 0;
        }
        //Debug.Log("current rotation" + currentRotation);
        if (currentRotation == 360) currentRotation = 0;

    }

    public void UndoRotateTetromino()
    {
        //blue doesnt rotate

        if (type == TetroMinoColor.Orange || type == TetroMinoColor.Cyan || type == TetroMinoColor.Red)
        {

            //tetrominos that toggle rotates 90 and 0
            if (currentRotation == 0)
            { RotateTetrominoMatrix(false); }
            else
            {
                RotateTetrominoMatrix(true);
            }

        }
        else if (type != TetroMinoColor.Blue)
        {
            //tetrominos that rotate 360 , 90 degrees each time
            RotateTetrominoMatrix(false);
        }

        //ResetTetroMino();
        UpdateTetromino();
        //RebuildTetromino(color);



    }

    public bool CheckTetrominoInGrid(Transform mino)
    {
        for (int y = 0; y < _playerGrid.height; y++)
        {
            for (int x = 0; x < _playerGrid.width; x++)
            {
                if (mino == _playerGrid.minosMatrix[x, y])
                    return true;
            }
        }

        return false;
    }

    public void UpdateMinosInGrid()
    {
        if (tetrominoMatrix == null) return;

        for (int row = 0; row < tetrominoMatrix.GetLength(0); row++)
        {
            bool removeRow = false;
            for (int col = 0; col < tetrominoMatrix.GetLength(1); col++)
            {

                Mino aux = GetMinoByIndex(row, col);
                if (aux != null)
                {
                    if (!CheckTetrominoInGrid(aux.transform))
                    {
                        removeRow = true;
                        continue;                        
                    }
                }
            }

            if (removeRow)
            {
                MoveMinoRowDown(row);
                RemoveLastMinosRow();// we've pulled the minos to the last line
                UpdateMinosInGrid(); // needs to check again the rows //so we can just remove the minos the last line
            }
        }
        RedrawTetromino();

    }

    void RemoveLastMinosRow()
    {
        int lastRow = tetrominoMatrix.GetLength(0) - 1;

        for (int col = 0; col < tetrominoMatrix.GetLength(1); col++)
        {// the line has switched so i want to remove the above one now
            Mino aux = GetMinoByIndex(lastRow, col);
            if (aux != null)
            {
                RemoveMinoFromMatrix(aux);
            }
        }
    }


    void MoveMinoRowDown(int r)
    {
        for (int col = 0; col < tetrominoMatrix.GetLength(1); col++)
        {
            MoveAboveMinosDownByCol(r, col);
        }
    }


    Mino GetMinoByIndex(int r, int c)
    {
        foreach (Mino m in minos)
        {
            if (m.row == r && m.col == c)
            { return m; }
        }

        return null;

    }

    void MoveAboveMinosDownByCol(int r, int c)

    {
        //Debug.Log("R" + r);
        for (int row = r + 1; row < tetrominoMatrix.GetLength(0); row++)
        {

            Mino aux = GetMinoByIndex(row - 1, c); //swaps the index of the minos
            Mino aux2 = GetMinoByIndex(row, c);

            if (aux != null) aux.row = row;
            if (aux2 != null) aux2.row = row - 1;
            tetrominoMatrix[row - 1, c] = tetrominoMatrix[row, c];
        }

        tetrominoMatrix[tetrominoMatrix.GetLength(0) - 1, c] = 0;
    }
   
    public void RemoveMinoFromMatrix(Mino mino)
    {
        minos.Remove(mino);
        mino.RemoveMinoObject(); //atenção so remover objeto depois de por a zero

    }

    /// <summary>
    /// Rotates a Target Tetromino matrix and returns the current rotation
    /// </summary>
    
    public void RotateTetromino()
    {
        //blue doesnt rotate

        if (type == TetroMinoColor.Orange || type == TetroMinoColor.Cyan || type == TetroMinoColor.Red)
        {
            //tetrominos that toggle rotates 90 and 0
            if (currentRotation == 0)
            {
                RotateTetrominoMatrix(true);
            }
            else
            {
                //Debug.Log("false rotation");
                RotateTetrominoMatrix(false);
            }

        }
        else if (type != TetroMinoColor.Blue)
        {
            //tetrominos that rotate 360 , 90 degrees each time
            RotateTetrominoMatrix(true);
        }

        UpdateTetromino();

    }




    void ReverseColumns(int[,] currentTetrominoMatrix)


    {
        int numRows = currentTetrominoMatrix.GetLength(0);
        int numCols = currentTetrominoMatrix.GetLength(1);

        for (int currentCol = 0; currentCol < numCols; currentCol++)
        {
            //num ciclo for podemos iniciar várias variaveis e varias condiºoes de paragem e varios incrementos
            //assim ele faz dois rows ao mesmo tempo so tem de fazer metade
            for (int currentRow = 0, targetRow = numRows - 1; currentRow < targetRow; currentRow++, targetRow--)
            {
                int temp = currentTetrominoMatrix[currentRow, currentCol];
                currentTetrominoMatrix[currentRow, currentCol] = currentTetrominoMatrix[targetRow, currentCol];
                currentTetrominoMatrix[targetRow, currentCol] = temp;


            }
        }

    }




    private void HideMinos()
    {
        foreach (Mino child in minos)
        {
            child.gameObject.SetActive(false);
        }
    }


    private void ResetParenting()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).SetParent(MinoPool.instance.poolParent.transform);
        }
    }


    public void ResetTetroMino()
    {
        //Percorre todos os minos
        //destroys all tetromino

        HideMinos();
        ResetParenting();
        //fica a faltar remover da matrix

    }
    

    public void RedrawTetromino()
    {

        foreach (Mino mino in minos)
        {
            mino.ReDrawMino();

        }
    }

    public void UpdateTetromino()
    {
        //updates without remaking 
        ResetTetroMino();
        DrawTetroMino();


    }

    public void ChangeTetrominoToNewType(TetroMinoColor type)
    {
        ResetTetroMino();
        tetrominoMatrix = null;
        Initialize(_playerGrid, type);
    }






    /// <summary>
    /// Method that draws and redraws the TetroMino acordingly to its matrix configuration
    /// </summary>
    private void DrawTetroMino()
    {
        minos = new List<Mino>();

        if (tetrominoMatrix == null)
        {
            throw new NullReferenceException();
        }
        //GetLength x e y
        for (int row = 0; row < tetrominoMatrix.GetLength(0); row++)
        {
            for (int col = 0; col < tetrominoMatrix.GetLength(1); col++)
            {

                if (tetrominoMatrix[row, col] == 1)
                {
                    //string type = GetMinoType(x, y); // gets the type of minoObject to be drawn
                    GameObject temp = MinoPool.instance.GetPooledMinoObject(); //instantiates a game object minoObject that has a sprite renderer
                    temp.SetActive(true);
                    temp.transform.SetParent(this.transform); // set this gameobject as parentFolder of the minos
                    try
                    {
                        temp.GetComponent<Mino>().Initialize(row, col, type, tetrominoMatrix); //the parentFolder must be attributed first}
                        minos.Add(temp.GetComponent<Mino>());
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);

                    }
                    temp.transform.localPosition = new Vector2(col, row); // set the minoObject position local
                    temp.transform.rotation = Quaternion.identity; // no rotation
                    temp.transform.name = "row" + row + "col" + col;


                }
               
            }


        }

    }
 

    


}
