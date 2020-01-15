using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Navigation : MonoBehaviour
{
    public static Navigation instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(this); }
    }
    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "Home")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                NavigateToScene("Level");

            }

        }

    }
    public void NavigateToScene(string scene)
    {

        SceneManager.LoadScene(scene);
    }
    
}
