using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Trash : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }



    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("GAME HAS BEEN QUIT");
    }
}
