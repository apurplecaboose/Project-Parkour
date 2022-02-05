using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliHover : MonoBehaviour
{
    public bool ismenu;
    bool backwards = false;
    float startingpos;
    float startingposy;
    public bool wongame = false;
    bool flyaway = false;

    void Start()
    {
        startingpos = transform.localPosition.x;
        startingposy = transform.localPosition.y;
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if (ismenu)
        {
            if (backwards)
            {
                transform.position -= new Vector3(10, 0, 10) * Time.deltaTime;
                if (transform.localPosition.x < startingpos - 75) backwards = false;
            }
            else
            {
                transform.position += new Vector3(10, 0, 10) * Time.deltaTime;
                if (transform.localPosition.x > startingpos + 75) backwards = true;
            }
        }
        else
        {
            if (backwards)
            {
                transform.position -= new Vector3(5, 0, 5) * Time.deltaTime;
                if (transform.localPosition.x < startingpos - 15) backwards = false;
            }
            else
            {
                transform.position += new Vector3(5, 0, 5) * Time.deltaTime;
                if (transform.localPosition.x > startingpos + 15) backwards = true;
            }
        }
        if(wongame)
        {
            transform.position += new Vector3(0, 30, 0) * Time.deltaTime;
            if (transform.localPosition.y > startingposy + 100) flyaway = true;
            if(flyaway)
            {
                transform.position += new Vector3(20, 0, 20) * Time.deltaTime;
            }
        }
    }
}
