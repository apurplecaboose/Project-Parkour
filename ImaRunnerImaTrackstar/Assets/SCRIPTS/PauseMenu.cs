using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject PauseMenuUI;
    public PlayerStats PlayerStats;
    public TMP_Text text;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                resume();
            else
                pause();
        }
        if (GameIsPaused)
        {
            text.SetText(PlayerStats.LookSens.ToString("0"));
        }
    }

    public void resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void MainMenu()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayerStats.CheckPointBool = false;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        PlayerStats.CheckPointBool = false;
        Application.Quit();
        Debug.Log("GAME HAS BEEN QUIT");
    }
    public void SensSlider(float newsens)
    {
        PlayerStats.LookSens = newsens;
    }
}
