using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public GameObject CarControl;
    public GameObject labManager;
    void Start()
    {
        StartCoroutine(startGame());
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(3f); // 3초후
        CarControl.GetComponent<VehicleControl>().enabled = true; // car컨트롤 스크립트 활성화하여 움직임가능
        labManager.GetComponent<LapTimeManager>().enabled = true; // 타임스타트
    }

}
