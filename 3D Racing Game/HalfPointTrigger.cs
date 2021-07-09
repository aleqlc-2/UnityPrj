using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPointTrigger : MonoBehaviour
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;

    void OnTriggerEnter()
    {
        // 하프랩을 거쳤을때만 최종지점에 도달했을때 카운트와 시간갱신 활성화
        LapCompleteTrig.SetActive(true);
        HalfLapTrig.SetActive(false);
    }
}
