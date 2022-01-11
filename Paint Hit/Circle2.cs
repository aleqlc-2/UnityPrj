using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle2 : MonoBehaviour
{
    void Start()
    {
        iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
        {
            "y",
            0,
            "easetype",
            iTween.EaseType.easeInOutQuad,
            "time",
            0.6,
            "OnComplete",
            "RotateCircle" // iTween끝나고 호출될 메서드명
        }));
    }

    private void RotateCircle()
    {
        // 0.8초동안 회전하고 0.4초 쉬었다가 반대로 0.8초동안 회전 반복
        iTween.RotateBy(base.gameObject, iTween.Hash(new object[]
        {
            "y",
            0.8f,
            "time",
            BallHandler.rotationTime,
            "easeType",
            iTween.EaseType.easeInOutQuad,
            "loopType",
            iTween.LoopType.pingPong,
            "delay",
            0.4
        }));
    }
}
