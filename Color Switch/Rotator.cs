using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 100f;

    void Update()
    {
        // Time.fixedDeltaTime 쓰면 동일한데
        // Time.time 쓰면 엄청빨리 회전함
        transform.Rotate(0f, 0f, Time.deltaTime * speed);
    }
}
