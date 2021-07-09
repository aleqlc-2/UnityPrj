using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public BallController target; // GameObject 자료형으로 해도됨
    private float offset;

    void Awake()
    {
        // 게임시작시 카메라의 y 와 볼의 y 의 차이를 고정값으로 설정
        offset = transform.position.y - target.transform.position.y;
    }

    void Update()
    {
        Vector3 curPos = transform.position;
        curPos.y = target.transform.position.y + offset;
        transform.position = curPos;
    }
}
