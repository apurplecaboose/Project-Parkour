using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WINNER : MonoBehaviour
{

    public GameObject Cake;
    public PlayerStats PlayerStats;
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Cake.SetActive(true);
            PlayerStats.CheckPointBool = false;
            PlayerStats.tutorialclear = true;
        }
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            Application.Quit();
        }
    }
}
