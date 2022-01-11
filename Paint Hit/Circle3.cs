using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle3 : MonoBehaviour
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

    void Update()
    {
        // Vector3.down이므로 y축을 기준으로 왼쪽으로 회전
        transform.Rotate(Vector3.down * Time.deltaTime * (BallHandler.rotationSpeed + 20));
    }
}
