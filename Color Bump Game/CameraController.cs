using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (GameManager.singleton.GameStarted)
            // deltatime은 똑같이 동작하지만 Time.time쓰면 카메라 엄청빨리 가버림
            // Time.deltaTime은 프레임간의 간격
            transform.position += Vector3.forward * Time.fixedDeltaTime * 5;
    }
}