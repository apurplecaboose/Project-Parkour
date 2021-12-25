using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamRocket : MonoBehaviour
{
    public Rigidbody Rb;
    public PlayerLookScript PLS;
    public float thrusters;

    private void OnTriggerEnter(Collider other)
    {
        if (PLS.xRot < 0f)
        {
            Rb.AddForce(Vector3.up * thrusters, ForceMode.Impulse);
        }
    }
}
