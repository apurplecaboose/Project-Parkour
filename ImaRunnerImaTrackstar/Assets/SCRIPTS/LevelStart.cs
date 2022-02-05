using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{

    bool ReadytoRumble = true;
    private void Awake()
    {
        Time.timeScale = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)| Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(ReadytoRumble)
            {
                ReadytoRumble = false;
                Time.timeScale = 1;
                Destroy(gameObject);
            }
        }
    }
}
