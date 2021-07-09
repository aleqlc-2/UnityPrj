using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Image health_Stats, stamina_Stats;

    public void Display_HealthStats(float healthValue)
    {
        healthValue /= 100f;
        health_Stats.fillAmount = healthValue; // fillAmount가 0~1임, 인스펙터창에서도 확인가능
    }

    public void Display_StaminaStats(float staminaValue)
    {
        staminaValue /= 100f;
        stamina_Stats.fillAmount = staminaValue;
    }
}
