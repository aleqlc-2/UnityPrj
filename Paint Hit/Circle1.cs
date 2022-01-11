using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle1 : MonoBehaviour
{
    void Start()
    {
        iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
        {
            "y",
            0,
            "easetype",
            iTween.EaseType.easeInCirc,
            "time",
            0.2,
            "OnComplete",
            "RotateCircle" // iTween끝나고 호출될 메서드명
        }));
    }

    void Update()
    {
        // y축을 기준으로 오른쪽으로 회전
        transform.Rotate(Vector3.up * BallHandler.rotationSpeed * Time.deltaTime);
    }

    private void RotateCircle()
    {
        print("The iTween anim is done");
    }
}
