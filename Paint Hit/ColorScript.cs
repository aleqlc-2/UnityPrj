using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScript : MonoBehaviour
{
    public static Color[] colorArray;

    public Color[] color1;
    public Color[] color2;
    public Color[] color3;

    void OnEnable()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        int randomC = Random.Range(0, 2);

        PlayerPrefs.SetInt("ColorSelect", randomC);
        PlayerPrefs.GetInt("ColorSelect");

        if (PlayerPrefs.GetInt("ColorSelect") == 0) colorArray = color1;
        if (PlayerPrefs.GetInt("ColorSelect") == 1) colorArray = color2;
        if (PlayerPrefs.GetInt("ColorSelect") == 2) colorArray = color3;
    }
}
