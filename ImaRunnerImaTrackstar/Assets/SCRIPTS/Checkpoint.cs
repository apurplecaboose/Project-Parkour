using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public Transform PlayerPos;


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
}
