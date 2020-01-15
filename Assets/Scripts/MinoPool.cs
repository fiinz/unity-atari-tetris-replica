using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MinoPool : MonoBehaviour
{
	public static MinoPool instance;//singleton
	public GameObject minoPrefab; //object to be pooled
	public  GameObject poolParent;
	private List<GameObject> pooledMinos;
	public int minosAmount;
	public bool willGrow = true;



	void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(this);
	}

	


	private void OnEnable()
	{
		pooledMinos = new List<GameObject>();

		for (int i = 0; i < minosAmount; i++)
		{
			GameObject newMino = (GameObject)Instantiate(minoPrefab);
			newMino.SetActive(false);
			newMino.transform.SetParent(poolParent.transform);
			pooledMinos.Add(newMino);

		}

	}
    public void DisableMino(Mino mino)
    {


        mino.gameObject.SetActive(true);
        mino.transform.SetParent(poolParent.transform);
        mino.transform.name = "";
        transform.position = Vector2.zero;

    }

    public GameObject GetPooledMinoObject()
	{

		for (int i = 0; i < pooledMinos.Count; i++)
		{
			//search is really fast thats why we use search
			if (!pooledMinos[i].activeInHierarchy) //if the mino isnt active retunr it
			{
                pooledMinos[i].GetComponent<Mino>().enabled=true;
                return pooledMinos[i];
			}

		}

		if (willGrow)
		{ // if it grows it creates a new object
			GameObject newMino = (GameObject)Instantiate(minoPrefab);
			pooledMinos.Add(newMino);
			return newMino;
		}

		return null; // we are out of objects
	}
	
}
