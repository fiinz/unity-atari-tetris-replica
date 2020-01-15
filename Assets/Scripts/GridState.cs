using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridState 
{
    InitializingGrid,
    InsertingNewTetromino,
    ProcessingRows,
    DeletingRows,
    SpawningNewTetromino,
    UpdatingTetroMinoInGrid,
    PoccessingLimitsExceded,
    ProcessingRoundComplete
}
