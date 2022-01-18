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
    public GameObject Cake;
    public GameObject NotCake;
    public PlayerStats PlayerStats;
    private bool Lvlcleared = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            TimerActive = true;
            Lvlcleared = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
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
