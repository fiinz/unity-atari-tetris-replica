using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoPool : MonoBehaviour
{
    public static TetrominoPool instance;
    public List<GameObject> pooledTetrominos;
    public int tetroMinosAmmount;
    public GameObject tetrominoPrefab;
    public Transform tetrominoParent;
    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }


    private void OnEnable()
    {
        pooledTetrominos = new List<GameObject>();

        for (int i = 0; i < tetroMinosAmmount; i++)
        {
            GameObject newTetroMino = (GameObject)Instantiate(tetrominoPrefab);
            newTetroMino.SetActive(false);
            newTetroMino.transform.SetParent(tetrominoParent.transform);
            pooledTetrominos.Add(newTetroMino);

        }

    }
  


    public GameObject GetPooledTetroMinoObject()
    {

        for (int i = 0; i < pooledTetrominos.Count; i++)
        {
            //search is really fast thats why we use search
            if (!pooledTetrominos[i].activeInHierarchy) //if the mino isnt active retunr it
            {return pooledTetrominos[i];}
        }
        return null; // we are out of objects

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
