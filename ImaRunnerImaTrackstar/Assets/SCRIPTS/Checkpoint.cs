using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public Transform PlayerPos;
    public PauseMenu PauseMenu;
    public WINNER winner;
    public AudioManager soundBitch;
    public GameObject Tooltips;

    private void Awake()
    {
        winner = FindObjectOfType<WINNER>();
        soundBitch = FindObjectOfType<AudioManager>();

        if (!PlayerStats.CheckPointBool)
        {
            PlayerPos.position = new Vector3(0, 6, -382);
            PlayerStats.yRot = 0f;
            Time.timeScale = 1;
        }
        else
        {
            PlayerPos.position = PlayerStats.ChkTransform;
            PlayerStats.yRot = PlayerStats.CheckPointRot;
        }
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            if (PlayerStats.tutorialclear)
            {
                Tooltips.SetActive(false);
            }
        }
    }
    public void Update()
    {
        if (Input.GetButtonDown("Reset") && winner.CurrentTime > 0)
        {
                Time.timeScale = 0;
                winner.CurrentTime = 0;
                RESETall_Checkpoints();
        }
    }
    public void RESETall_Checkpoints()
    {
        soundBitch.Play("ButtonSfx");
        PlayerStats.LookType = 1;
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
  
        PlayerStats.CheckPointBool = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RESETLastCheckpoint()
    {
        soundBitch.Play("ButtonSfx");
        PlayerStats.LookType = 1;
        PauseMenu.GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }




    /* public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerStats.ChkTransform = PlayerPos.position;
            PlayerStats.CheckPointBool = true;
            PlayerStats.CheckPointRot = transform.rotation.eulerAngles.y;
        }
    }
    */
}
