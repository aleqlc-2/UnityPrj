using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneSleepMode : MonoBehaviour
{
    void Start()
    {
        // 사용자의 입력없어도 절전모드 안되도록
        // Screen.sleepTimeout = 30; 이렇게쓰면 30초간 입력없으면 절전모드
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
