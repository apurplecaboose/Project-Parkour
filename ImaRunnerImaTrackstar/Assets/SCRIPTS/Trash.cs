using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Trash : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public void StartGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        if (PlayerStats.tutorialclear)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }
    public void OptionsMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(1);
    }
    public void ReturntoSplash()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(0);
    }
    public void Tutorial()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(3);
    }
    public void Lv1()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(4);
    }
    public void Lv2()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(5);
    }
    public void Lv3()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        SceneManager.LoadScene(6);
    }
    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        Application.Quit();
        Debug.Log("GAME HAS BEEN QUIT");
    }
}
