using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public GameObject PauseMenuParent;
    bool ReadytoRumble = true;
    private void Awake()
    {
        PauseMenuParent = GameObject.Find("PauseMenuParent");
        PauseMenuParent.SetActive(false);
        Time.timeScale = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)| Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(ReadytoRumble)
            {
                PauseMenuParent.SetActive(true);
                ReadytoRumble = false;
                Time.timeScale = 1;
                Destroy(gameObject);
            }
        }
    }
}
