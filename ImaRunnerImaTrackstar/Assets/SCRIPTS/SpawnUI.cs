using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUI : MonoBehaviour
{
    public GameObject UIELEMENT;
    private void Awake()
    {
        UIELEMENT.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        UIELEMENT.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        UIELEMENT.SetActive(false);
    }
}
