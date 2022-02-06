using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Slider slider;
    public PlayerStats TheStats;
    public Gradient gradient;
    public Image Fillcolor;
    public TMP_Text speedtext;
    public void SetSpeed(float speed)
    {
        slider.value = speed;
        Fillcolor.color = gradient.Evaluate(slider.normalizedValue);
    }
    
    void Update()
    {
        SetSpeed(TheStats.HorzSpeedVal);
        //TheStats.HorzSpeedVal.ToString("0.00");
        speedtext.SetText(TheStats.HorzSpeedVal.ToString("0.00"));
    }
}
