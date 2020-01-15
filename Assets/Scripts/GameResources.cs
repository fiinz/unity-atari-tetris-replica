using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources:MonoBehaviour
{

    public static GameResources instance;
    [SerializeField]
    private Sprite[] minoSprites;

    public void Awake()
    {
        if (instance == null) { instance = this; LoadMinos(); }
        else { Destroy(this); }
    }

    public void LoadMinos()
    {
        minoSprites = Resources.LoadAll<Sprite>("Minos/Minos");

    }
    public Sprite GetSpriteByName(string name) //get the sprite from a given name
    {
        if (minoSprites != null)
        {
            foreach (Sprite mino in minoSprites)
            {
                if (mino.name == name) { return mino;}

            }
        }
        return null; 
    }
    // Use this for initialization
  
}
