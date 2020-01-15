using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;
using UnityEngine.Serialization;

/// <summary>
///  Manages a Transform Grid for a given Player 
/// </summary>
public class PlayerGrid : MonoBehaviour
{
   
    public int height;
    public int width;

    //grid delegates and events
    public delegate void GridHandler();
//    public event GridHandler TetrominoSpawnEvent;
    public event GridHandler GridProcessedEvent;
    public event GridHandler FullTetrisCompletedEvent;
    public event GridHandler FullGridEvent;

    public delegate void GridRowHandler(int rows);
    public event GridRowHandler GridLineCompleted;



    public Transform[,] minosMatrix; //the grid were the Tranform Minos will Be Referenced
    public Vector2 nextTetrominoPos; //nextTetrominoPlaceholders
    private Tetromino nextTetrominoPreviw; //next Tetromino Preview
    private TetroMinoColor nextTetroMinoType;
    public Tetromino currentTetromino; // the current tetromino reference
    public TetrominoMovement currentTetrominoMovement;
    public List<Tetromino> tetrominosList;
    public int spawnedTetrominos = 0;
    public Player player;
    private bool isInitialized;
    private Vector2 spawnPoint;
    public GridState gridState;
    public bool spawnBlocks = false;

    public List<AudioClip> sfx;


    public void SetPreviewTetromino(Vector3 nextTetrominoPos)
    {
        var tetrominoObject = TetrominoPool.instance.GetPooledTetroMinoObject();
        nextTetrominoPreviw = tetrominoObject.GetComponent<Tetromino>();
        nextTetrominoPreviw.Initialize(this, GetRandomTetroMinoType());
        nextTetrominoPreviw.transform.name = "PreviewNext";
        nextTetrominoPreviw.transform.position = nextTetrominoPos;

    }
    public void SetEvents()
    {
        GridLineCompleted += ProcessLinesScore;
        GridLineCompleted += player.round.ProcessLinesCompleted;

        GridProcessedEvent += PlayTetrominoDropSound;
        GridProcessedEvent += CheckNextSpawn;
        player.round.RoundCompleteEvent += ProcessRoundComplete;

        FullTetrisCompletedEvent += ProcessFullTetris;
        FullGridEvent += ProcessGameOver;
      

        if (player.currentRound >= 7)
        {
            spawnBlocks = true;
            GridProcessedEvent += SpawnBlock;
        }

    }

    public void ProcessRoundComplete(int row)
    {

        gridState = GridState.ProcessingRoundComplete;

    }
    public void OnDisable()
    {

        GridProcessedEvent -= SpawnNewTetrominoIntoGrid;
        FullGridEvent -= ProcessGameOver;
        FullTetrisCompletedEvent -= ProcessFullTetris;
        GridLineCompleted -= player.round.ProcessLinesCompleted;

    }

    public void CheckNextSpawn()
    {
        if (!isGridFull())
        {
            if (player.round.state==global::Round.RoundState.Completed)
            {

                gridState = GridState.ProcessingRoundComplete;
            }
            else
            {

                SpawnNewTetrominoIntoGrid();
            }
            
        }
        else
        {
            gridState = GridState.PoccessingLimitsExceded;
            CallFullGridEvent();
           
           
        }


    }
    public bool isGridFull()
    {

        for (int x = 0; x < width; x++)
        {
            if (minosMatrix[x, 20]!=null)
            {
                Debug.Log("Checking Last Row");
                return true;
            }

        }

        return false;
    }
    public void Initialize(Player player, Vector3 nextTetrominoPos, Vector3 spawnPoint, int width, int height)
    {
        if (!isInitialized)
        {
            this.spawnPoint = spawnPoint;
            this.player = player;
            this.isInitialized = true;
            this.width = width;
            this.height = height;
            //default values in tetris playerGrid
            minosMatrix = new Transform[width, height];
            SetPreviewTetromino(nextTetrominoPos);
            SetEvents();
            gridState = GridState.InitializingGrid;
           
            SpawnNewTetrominoIntoGrid();
            player.currentGrid = this;
            if (player.currentRound>=4 && player.currentRound<7)
            {
                //desenhar as colunas
            }
           
        }

    }
    public bool LastEmptyCellInRow(int row)
    {
        int count = 0;
        
        for (int x = 0; x < width; x++)
        {
            if (minosMatrix[x,row] == null) count++;

            if (count > 0) return false;
        }

        return true;
    }
   
    public void SpawnBlock()
    {
        var value = UnityEngine.Random.Range(0, 101);
        if(value<=(50+player.currentRound+50))
        {
            bool valid = false;
            while (!valid)
            {
                int x = UnityEngine.Random.Range(0, 10);
                int y = UnityEngine.Random.Range(0, 20);
                if (minosMatrix[x,y] == null)
                {
                    if (y != 0)
                    {
                        if (minosMatrix[x,y - 1] != null && !LastEmptyCellInRow(y))
                        {
                            valid = true;
                            InsertSingleTetrominoAtPos(new Vector2(x,y),TetroMinoColor.Gray);


                        }
                    }
                  

                }

            }
        }


    }
    public void PlayTetrominoDropSound()
    {
        var sfxclip = sfx.Find(clip => clip.name.Contains("drop"));
        Sound2DManager.instance.PlaySfx(sfxclip);

    }
    public void ProcessMoveScore(int rows)

    {

        player.stats.UpdatePlayerScore(100);


    }
    public void ProcessLinesScore(int rows)
    {
        //
        player.stats.UpdatePlayerScore(rows * 100);
            //Debug.Log("Completed Sound");
       

    }


 
    public void CallFullGridEvent()
    {
        if (FullGridEvent != null) FullGridEvent();


    }
    public void CallGridProcessed()
    {
        if (GridProcessedEvent != null) GridProcessedEvent();
    }

    public void CallLinesDeleted(int lines)
    {
        if (GridLineCompleted != null) { GridLineCompleted(lines); }
    }


    public void CallFullTetrisRowsEvent()
    {
        if (FullTetrisCompletedEvent != null) FullTetrisCompletedEvent();
    }

    public int GetHighestRowWithMinos()
    {
        for (int y = height-1; y >0 ; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (minosMatrix[x, y] != null)
                {
                    return y;
                }

            }

        }
        return 0;
    }

    public void ProcessLevelComplete()
    {



    }
      public void ProcessFullTetris()
    {

        Debug.Log("process full tetris");

    }

    public void ProcessGameOver()
    {
        gridState = GridState.PoccessingLimitsExceded;
       // ClearGrid();
        Debug.Log("GAME OVER");

    }

    public void Update()
    {

        if (!isInitialized) { return;  }
           
        
       

    }
    private void ClearGrid()
    {
        foreach(var tetromino in tetrominosList)
        {
           tetromino.ClearTetromino();
        }

    }

    private void InsertSingleTetrominoAtPos(Vector2 pos,TetroMinoColor tetroMinoType)
    {

        var tetrominoObject = TetrominoPool.instance.GetPooledTetroMinoObject();
        Tetromino tetromino = tetrominoObject.GetComponent<Tetromino>();
        tetromino.Initialize(this, tetroMinoType);
        tetromino.transform.SetParent(this.gameObject.transform);
        tetromino.transform.localPosition = pos;
        tetrominoObject.name = "Block";
        tetrominosList.Add(tetromino);
        //tetrominoObject.GetComponent<TetrominoMovement>();
        //needs to check playerGrid 
        UpdateTetrominoInGrid(tetromino);
        var sfxclip = sfx.Find(clip => clip.name.Contains("spawn"));
        Sound2DManager.instance.PlaySfx(sfxclip);





    }

 
    /*
    private void ProcessDeletedRows(int rows)
    {
        Debug.Log("Fazer qualquer coisa aqui");
        SpawnNewTetrominoIntoGrid();

    }
    */
    private TetroMinoColor GetRandomTetroMinoType()
    {
        //Debug.Log(Game_References.instance);
        Array values = Enum.GetValues(typeof(TetroMinoColor));
        TetroMinoColor random = (TetroMinoColor)values.GetValue(UnityEngine.Random.Range(0, 7));

        return random;
    }

    private void ProcessNextTetromino() //next Tetromino will appear for each pfPlayer every time a Tetromino is Placed
    {
        nextTetroMinoType = GetRandomTetroMinoType();
        nextTetrominoPreviw.ChangeTetrominoToNewType(nextTetroMinoType);

    }


    public void SpawnNewTetrominoIntoGrid()
    {

        if (gridState != GridState.InsertingNewTetromino && gridState != GridState.PoccessingLimitsExceded
            && gridState!=GridState.ProcessingRoundComplete
            )

        {
            gridState = GridState.InsertingNewTetromino;
            var tetrominoObject = TetrominoPool.instance.GetPooledTetroMinoObject();
            currentTetromino = tetrominoObject.GetComponent<Tetromino>();
            currentTetromino.transform.SetParent(this.gameObject.transform);
            currentTetromino.Initialize(this, nextTetroMinoType);
            currentTetromino.transform.localPosition = spawnPoint;
            currentTetromino.name = "Tetromino" + spawnedTetrominos;
            tetrominosList.Add(currentTetromino.GetComponent<Tetromino>());
            spawnedTetrominos++;
            player.round.IncrementTetroMinosSpawned(nextTetroMinoType);
            ProcessNextTetromino();
            currentTetrominoMovement = currentTetromino.GetComponent<TetrominoMovement>();
            currentTetrominoMovement.Initialize(this, currentTetromino);
            

        }

    }





    public void ClearTetrominoFromGrid(Tetromino tetromino)
    {
        // it should only update playerGrid
        // updates the playerGrid 
        //first checks all the playerGrid to clear the tetromino'minos from the playerGrid

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (minosMatrix[x, y] != null)
                {
                    if (minosMatrix[x, y].parent == tetromino.transform) //se o mino que está na playerGrid pertencer ao tetromino  //coloca o spot a null todos os minos deste tetromino vao mudar de posiçcao
                    {
                        minosMatrix[x, y] = null; //faz o update colocando a null
                    }
                }
            }
        }


    }

    /// <summary>
    /// removes a tetromino from grid ( clears ) and update its position
    /// </summary>
    /// <param name="tetromino"></param>

    public void UpdateTetrominoInGrid(Tetromino tetromino)
    {
        gridState = GridState.UpdatingTetroMinoInGrid;
        ClearTetrominoFromGrid(tetromino);
        UpdateTetrominoPositionInGrid(tetromino);
    }

    public void UpdateAllTetrominosInGrid()
    //updates all tetrominos in the grid its from and if minos are not alocated in the grid
    {
        foreach (Tetromino tetromino in tetrominosList)
        {
            //Debug.Log("updating tetrominos");
            tetromino.UpdateMinosInGrid();

        }


    }

    public void UpdateTetrominoPositionInGrid(Tetromino tetromino)
    {

        //now updates the new position for this tetromino minos
        foreach (Transform mino in tetromino.transform)
        {
            // i need to add the local position of the tetromino and the mino so i can pu tthem on the playerGrid
            // ot
            Vector2 pos = tetromino.transform.localPosition + mino.localPosition;
            //Debug.Log("mino pos x" + pos.x + "y" + pos.y);
            if (pos.y < height)
            {
                //only checks in Y
                minosMatrix[(int)pos.x, (int)pos.y] = mino;
            }

        }

    }



    /// <summary>
    /// It Rounds each coordinate of the passed Vector
    /// </summary>
    /// <param name="pos"> The vector wich positions should be rounded</param>
    /// <returns></returns>
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public bool CheckIfNextRotationIsPossible(Tetromino tetromino)
    {


        Vector2 pos = Vector2.zero;
        tetromino.RotateTetromino();

        //here it checks for each mino inside tetromino and check every position
        foreach (Transform mino in tetromino.transform)
        {
            pos = tetromino.transform.localPosition + mino.localPosition;

            if (!CheckIsInsideGrid(pos))
            {
                tetromino.UndoRotateTetromino();
                return false;
            }


            //checks if theres already a mino on that position and if 
            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
            {
                tetromino.UndoRotateTetromino();
                return false;
            }

        }
        tetromino.UndoRotateTetromino();
        return true;
    }
    public bool NextMoveIsPossible(Tetromino tetromino, Vector2 nextMove)
    {

        Vector2 pos = Vector2.zero;


        //here it checks for each mino inside tetromino and check every position
        foreach (Transform mino in tetromino.transform)
        {
            // for each mino inside of the tetromino


            pos = Round(tetromino.transform.localPosition + mino.localPosition) + nextMove;

            //checks next move and verifies every mino pos
            //Debug.Log("pos" + pos); //checks if every mino is inside of the playerGrid
            if (!CheckIsInsideGrid(pos)) return false;


            //checks if theres already a mino on that position and if 
            if (GetTransformAtGridPosition(pos) != null
                && GetTransformAtGridPosition(pos).parent != tetromino.transform)
            {
                return false;
            }

        }
        return true;
    }

    /// <summary>
    /// checks if the given position is inside of the playerGrid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < width && (int)pos.y >= 0);
    }

    public bool CheckIsAboveGrid(Tetromino tetromino)
    {

        for (int x = 0; x < width; ++x)
        {
            foreach (Transform mino in tetromino.transform) //check each minoObject pos
            {
                Vector2 pos = mino.position;
                if (pos.y > height - 1)
                {
                    //theres a minoObject above
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// //verifies if theres already a mino on a given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {

        if (pos.y >= height)
        { return null; }
        else
        { return minosMatrix[(int)pos.x, (int)pos.y]; }

    }


    public bool CheckIsBottom(Vector2 pos)
    {
        return ((int)pos.y == 0);
    }


    public void UpdateMinos()
    {
        /*for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if (grid[x, y] != null) //if theres an object on the playerGrid
				{
					//if (playerGrid[x, y].parent.GetComponent<Tetromino>() == null) Debug.Log(playerGrid[x, y]);
					var tempMino = grid[x, y].GetComponent<Mino>();
					tempMino.tetroMinoParent.UpdateTetromino();
					//é necessario fazer o rebuild tb da matriz e retirar o mino da matriz


				}
			}

		}*/

    }

    /// <summary>
    /// Delete a row of Minos
    /// </summary>
    /// <param name="y">the row index to delete</param>

 
    public void DeleteGridRowAt(int y)
    {
        for (int x = 0; x < width; x++)
        {
            minosMatrix[x, y] = null;
        }
    }
    /// <summary>
    /// Moves Down a Row Setting the Above One to Null
    /// </summary>
    /// <param name="y"> the Row to Move Down</param>
    public void MoveRowDown(int y)
    {

        for (int x = 0; x < width; x++) //vai apagar a linha toda
        {
            if (minosMatrix[x, y] != null) // se os minos da linha atual não é null 
            {
                ////copies the row ( the minos )to the line below
                minosMatrix[x, y - 1] = minosMatrix[x, y];
                //deletes the atual one ( the one that is being moved down)
                minosMatrix[x, y] = null;


                //e movo o conteudo da linha para baixo
                //playerGrid[x, y-1].position+= new Vector3(0, -1, 0);
                //pois ele move de facto os tetrominos que decheram na matriz mas que ficaram em cima
                // e agora a nova referencia já é Y-1 porque eles desceram
                minosMatrix[x, y - 1].Translate(0, -1, 0, Space.World);


            }
        }

    }
    public void MoveTopRowsDown(int y) //iterates all the minos from a specific y to the height and move them down
    {
        for (int i = y; i < height; ++i)
        {
            //move cada uma das linhas ( onde aconteceu a linha );
            MoveRowDown(i);


        }
    }
    /// <summary>
    /// This function checks if the row is full of minos
    /// </summary>
    /// <param name="y"> the row index to check </param>
    /// <returns></returns>
    public bool CheckIfRowIsFullAt(int y)
    {
        for (int x = 0; x < width; x++)
        {
            //verifca se não tem objetos a ocupar aquele lugar
            if (minosMatrix[x, y] == null) return false; //if it finds a spot thats its null it returns false.
        }

        return true;
    }
    /// <summary>
    /// Check Grid and counts Lines that are complete
    /// </summary>
    /// 
    public void CheckRowsForClear()
    {
        gridState = GridState.ProcessingRows;

        int count = 0;
        for (int y = 0; y < height; ++y)
        {
            if (CheckIfRowIsFullAt(y))
            {
                count++;
            }

        }

        if (count > 0) //if it found rows start to delete
        {
            if (count == 4)
            {
                Debug.LogError("nao existe evento de tetris");
                FullTetrisCompletedEvent(); //fires an event of full tetris
                                               //Debug.LogWarning("needs to update score");
            }
            if (gridState == GridState.ProcessingRows)
            {
                StartCoroutine(RemoveCompletedRows(count, 0)); //using recursive clear Rows so it can update the moving rows

            }


        }
        else
        {
            if (gridState == GridState.ProcessingRows)
            {
                //nao fez nennuma
                CallGridProcessed();
            }
        }


    }


    public void AnimateDeletingRow(int row)
    {
        for (int x = 0; x < width; x++)
        {

            //tempMino.transform.name = "TO REMOVE";
            //var tempMino = grid[x, y].GetComponent<Mino>();
            //tempMino.transform.GetComponent<SpriteRenderer>().color=new Color(0.5f,0.2f,0.2f);
            //tempMino.RemoveMinoObject();
            //continue;

            //destroys the Mino not the tetromino
            minosMatrix[x, row].GetComponent<Mino>().isBeingRemoved = true;
        }



    }
    IEnumerator RemoveCompletedRows(int count, int deletedrows)
    {
        gridState = GridState.DeletingRows;

        for (int y = height - 1; y >= 0; y--)
        {

            if (CheckIfRowIsFullAt(y))
            {
                Debug.Log("y" + y);
                AnimateDeletingRow(y);
                var sfxclip = sfx.Find(clip => clip.name.Contains("completed"));
                Sound2DManager.instance.PlaySfx(sfxclip);
                yield return new WaitForSeconds(0.2f);
                DeleteGridRowAt(y); //remove os minos na linha y que estava cheia
                MoveTopRowsDown(y + 1);
                UpdateAllTetrominosInGrid();
                count--;
                deletedrows++;
                CallLinesDeleted(1);
               // StartCoroutine(RemoveCompletedRows(count, deletedrows)); //using recursive clear Rows so it can update the moving rows

            }
        }

        //CallRowCompletedEvent();

        if (count == 0)
        {
            UpdateAllTetrominosInGrid();
            CallGridProcessed();
        }

    }







}

