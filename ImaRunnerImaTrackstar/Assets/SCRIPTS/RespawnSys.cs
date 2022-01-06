using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnSys : MonoBehaviour
{
    private int CurrentSceneNumb;
    public Transform PlayerPos;


    void Start()
    {
        CurrentSceneNumb = SceneManager.GetActiveScene().buildIndex;
    }


    void Update()
    {
        if (PlayerPos.position.y < -10f)
        {
            SceneManager.LoadScene(CurrentSceneNumb);
        }
    }
}
