using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infinitewater : MonoBehaviour
{
    public Transform player;
    public bool rain;


    void Update()
    {
        if(!rain)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        }
        else
        {
            transform.position = new Vector3(player.position.x, player.position.y + 50f, player.position.z);
        }
    }
}
