using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public PlayerStats TheStats;
    public int furrycounter = 0;
    
    
    public void IGNtenOutofTenStars()
    {
        furrycounter += 1;
    }
    

    void Update()
    {
        if(furrycounter>25)
        {
            TheStats.tutorialclear = true;
        }
    }
}
