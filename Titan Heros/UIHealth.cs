using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    public Image health_Slider;

    public void DisplayHealthValue(float value)
    {
        value /= 100f;
        if (value < 0) value = 0f;

        health_Slider.fillAmount = value;
    }
}
