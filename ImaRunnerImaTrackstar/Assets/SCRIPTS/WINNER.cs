using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WINNER : MonoBehaviour
{
    public static WINNER instance;

    public float CurrentTime = 0f;
    bool TimerActive = true;
    GameObject Cake;
    GameObject NotCake;
    public PlayerStats PlayerStats;
    private bool Lvlcleared = false;
    public GameObject PlayerSmovement;
    public GameObject StartingUIElement;
    public GameObject ControlsUIElement;
    public GameObject SwapUI;
    public HeliHover HeliHover;
    public Animator ani;



    private void Awake()
    {
        Cake = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        NotCake = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        

        if (instance == null)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 | SceneManager.GetActiveScene().buildIndex == 1 | SceneManager.GetActiveScene().buildIndex == 2)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                TimerActive = true;
                Lvlcleared = false;
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 2 && !PlayerStats.CheckPointBool)
        {

            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                if (PlayerStats.tutorialclear)
                {
                    StartingUIElement.SetActive(true);
                }
                else
                {
                    ControlsUIElement.SetActive(true);
                }
            }
            else
            {
                ControlsUIElement.SetActive(true);
            }

        }
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if(CurrentTime <= PlayerStats.Leveltime)
            {
                TimerActive = false;
                PlayerStats.CheckPointBool = false;
                PlayerStats.tutorialclear = true;
                StartCoroutine(LevelCleared());
            }
            else
            {
                TimerActive = false;
                PlayerStats.CheckPointBool = false;
                StartCoroutine(LevelFailed());
            }
        }
    }
    private void Update()
    {

        if (SceneManager.GetActiveScene().buildIndex == 0 | SceneManager.GetActiveScene().buildIndex == 1 | SceneManager.GetActiveScene().buildIndex == 2)
        {
            Destroy(gameObject);
        }
        if (SwapUI == null)
        {
            SwapUI = GameObject.Find("SwapUI");
        }

        if (Time.timeScale == 0)
        {
            SwapUI.SetActive(false);
        }
        else
        {
            SwapUI.SetActive(true);
        }
        /////////
        if (TimerActive)
        {
            CurrentTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(CurrentTime);
            PlayerStats.CurrTime = time.Minutes.ToString() + ":" + time.Seconds.ToString("00") + ":" + time.Milliseconds.ToString("00");
        }
        if(Lvlcleared)
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                SceneManager.LoadScene(0);
            }
        }
    }
    IEnumerator LevelCleared()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<BoxPlayerMovement>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookScript>().enabled = false;
        GameObject.Find("PauseMenuParent").SetActive(false);
        ani = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<Animator>();
        HeliHover = GameObject.FindGameObjectWithTag("Helicopter").GetComponent<HeliHover>();
        HeliHover.wongame = true;
        ani.SetTrigger("Extract");
        Lvlcleared = true;
        yield return new WaitForSeconds(1);
        Cake.SetActive(true);
    }
    IEnumerator LevelFailed()
    {
        NotCake.SetActive(true);
        yield return new WaitForSeconds(3);
        PlayerStats.LvlFailed = true;
        Destroy(gameObject);
    }
}
