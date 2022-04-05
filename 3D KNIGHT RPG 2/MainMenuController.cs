using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void SetQuality()
    {
        ChangeQualityLevel();
    }

    private void ChangeQualityLevel()
    {
        string level = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        switch (level)
        {
            case "Low":
                QualitySettings.SetQualityLevel(0);
                break;

            case "Normal":
                QualitySettings.SetQualityLevel(1);
                break;

            case "High":
                QualitySettings.SetQualityLevel(2);
                break;

            case "Ultra":
                QualitySettings.SetQualityLevel(3);
                break;

            case "No Shadows":
                if (QualitySettings.shadows == ShadowQuality.All)
                    QualitySettings.shadows = ShadowQuality.Disable;
                else
                    QualitySettings.shadows = ShadowQuality.All;
                break;
        }
    }

    public void SetResolution()
    {
        ChangeResolution();
    }

    private void ChangeResolution()
    {
        string index = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        switch (index)
        {
            case "0":
                Screen.SetResolution(1152, 648, true);
                break;

            case "1":
                Screen.SetResolution(1280, 720, true);
                break;

            case "2":
                Screen.SetResolution(1360, 768, true);
                break;

            case "3":
                Screen.SetResolution(1920, 1080, true);
                break;
        }
    }
}
