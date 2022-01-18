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
            PlayerStats.yRot = 0f;
        }
        else
        {
            PlayerPos.position = PlayerStats.ChkTransform;
            PlayerStats.yRot = PlayerStats.CheckPointRot;
        }
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerStats.ChkTransform = PlayerPos.position;
            PlayerStats.CheckPointBool = true;
            PlayerStats.CheckPointRot = transform.rotation.eulerAngles.y;
        }
    }
    public void RESETall_Checkpoints()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        FindObjectOfType<WINNER>().CurrentTime = 0f;
        PlayerStats.LookType = 1;
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        PlayerStats.CheckPointBool = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RESETLastCheckpoint()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSfx");
        PlayerStats.LookType = 1;
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
