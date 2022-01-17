using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapZONE : MonoBehaviour
{
    public GameObject SwapUIHere;
    public GameObject NormalUIHere;
    public bool VertSwap;
    public bool HorzSwap;
    public bool ChaosSwap;

    public PlayerStats PlayerStats;

    private void Awake()
    {
        PlayerStats.LookType = 1;
    }
    private void OnTriggerEnter(Collider other)
    {
        SwapUIHere.SetActive(true);
        NormalUIHere.SetActive(false);
        if(HorzSwap)
        {
            PlayerStats.LookType = 2;
        }
        else if (VertSwap)
        {
            PlayerStats.LookType = 3;
        }
        else if(ChaosSwap)
        {
            PlayerStats.LookType = 4;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        NormalUIHere.SetActive(true);
        SwapUIHere.SetActive(false);

        PlayerStats.LookType = 1;
    }
}
