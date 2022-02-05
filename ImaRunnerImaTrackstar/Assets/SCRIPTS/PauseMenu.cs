using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;
    GameObject PauseMenuUI;
    public PlayerStats PlayerStats;
    public TMP_Text text;
    public AudioSource Audio;
    public AudioSource pausemus;

    private void Awake()
    {
        PauseMenuUI = transform.GetChild(0).gameObject;
    }


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
        GameObject.FindGameObjectWithTag("Player").GetComponent<BoxPlayerMovement>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookScript>().enabled = true;
        Audio.Play();
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void pause()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<BoxPlayerMovement>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookScript>().enabled = false;
        pausemus.Play();
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void MainMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        PlayerStats.LookType = 1;
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayerStats.CheckPointBool = false;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        PlayerStats.LookType = 1;
        PlayerStats.CheckPointBool = false;
        Application.Quit();
        Debug.Log("GAME HAS BEEN QUIT");
    }
    public void SensSlider(float newsens)
    {
        PlayerStats.LookSens = newsens;
    }
}
