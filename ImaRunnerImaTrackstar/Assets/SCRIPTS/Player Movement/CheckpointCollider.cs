using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCollider : MonoBehaviour
{

    public PlayerStats PlayerStats;
    public Transform PlayerPos;

    public AudioManager soundBitch;


    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerStats.ChkTransform = PlayerPos.position;
            PlayerStats.CheckPointBool = true;
            PlayerStats.CheckPointRot = transform.rotation.eulerAngles.y;
        }
    }
}
