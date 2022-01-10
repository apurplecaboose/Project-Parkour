using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public Transform PlayerPos;
    public PauseMenu PauseMenu;

    private void Awake()
    {
        if (!PlayerStats.CheckPointBool)
        {
            PlayerPos.position = new Vector3(0, 6, -382);
        }
        else
        {
            PlayerPos.position = PlayerStats.ChkTransform;
        }
        
        /*
        switch (PlayerStats.CheckPointNumber)
        {
            case 0:
                PlayerPos.position = new Vector3(0, 6, -382);
                break;
            case 1:
                PlayerPos.position = new Vector3(0, 0, 0);
                break;
        }
        */
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerStats.ChkTransform = PlayerPos.position;
            PlayerStats.CheckPointBool = true;
        }
    }
    public void RESETall_Checkpoints()
    {
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        PlayerStats.CheckPointBool = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RESETLastCheckpoint()
    {
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
