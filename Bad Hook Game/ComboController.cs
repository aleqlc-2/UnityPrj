using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboController : MonoBehaviour
{
    public static ComboController instance;
    public bool isComboActive;
    public int comboCounter;
    public int maxCombo;
    public Text txtCombo;

    public Animator comboAnim;

    [Range(0f, 5f)] public float comboTime;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isComboActive = false;
        comboCounter = 1;
    }

    public int AddCombo(int score, int addedScore)
    {
        if (isComboActive)
        {
            comboCounter++;
            CancelInvoke("StopCombo"); // 콤보가 시작되었으므로 StopCombo 종료

            // comboTime후에 StopCombo 호출되므로 그전까지 comboCounter++ 된만큼 콤보점수적용
            Invoke("StopCombo", comboTime);
        }
        else
        {
            StartCombo(); // 콤보중이아니면 새로운 콤보시작
        }

        comboAnim.Play("Combo Beat", 0, 0);
        comboCounter = Mathf.Clamp(comboCounter, 1, maxCombo);
        txtCombo.text = "x" + comboCounter;

        if (comboCounter == 1)
        {
            return score; // 처음 hit은 콤보로 인한 추가점수 미적용
        }

        return score + (addedScore * comboCounter); // 100 * 콤보개수로 추가점수
    }

    public void StartCombo()
    {
        isComboActive = true;
    }

    public void StopCombo()
    {
        isComboActive = false;
        comboCounter = 1;
    }
}
