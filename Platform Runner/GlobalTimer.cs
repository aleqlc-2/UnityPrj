using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalTimer : MonoBehaviour
{
    public GameObject timeDisplay01;
    public GameObject timeDisplay02;

    public bool isTakingTime = false;
    public int theSeconds = 150;

    public static int extendScore;

    void Update()
    {
        extendScore = theSeconds;

        if (isTakingTime == false)
        {
            StartCoroutine(SubtractSecond());
        }
    }

    IEnumerator SubtractSecond()
    {
        isTakingTime = true; // 일단 1초간 코루틴호출 멈추도록
        theSeconds -= 1; // 1빼고
        timeDisplay01.GetComponent<Text>().text = "" + theSeconds;
        timeDisplay02.GetComponent<Text>().text = "" + theSeconds;
        yield return new WaitForSeconds(1); // 1초 후
        isTakingTime = false; // 다시 코루틴호출
    }
}
