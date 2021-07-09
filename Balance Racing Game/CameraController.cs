using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    void FixedUpdate()
    {
        Vector3 newPosition = target.position; // car의 z가 0이라 z는 0이 할당됨
        newPosition.z = -10; // 카메라의 z는 -10이므로

        transform.position = newPosition;
    }
}
