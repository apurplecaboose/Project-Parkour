using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WINNER : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Cake;
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Cake.SetActive(true);
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
