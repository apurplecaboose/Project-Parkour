using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStats : MonoBehaviour
{
    public PlayerStats PlayerStats;

    private void Awake()
    {
        PlayerStats.Leveltime = 2700;
    }
    void Start()
    {
        PlayerStats.Leveltime = 2700;
    }
}
