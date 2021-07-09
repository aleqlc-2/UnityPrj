using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ball에 들어갈 스크립트
public class Bowling : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // ball이 아래로 떨어지면
        if (transform.position.y < -5f)
        {
            // ball을 다시 던질 수 있게 위치와 velocity 초기화
            transform.position = startPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
