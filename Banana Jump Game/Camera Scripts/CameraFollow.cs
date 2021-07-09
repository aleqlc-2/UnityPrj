using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    private bool followPlayer;

    public float min_Y_Threshold = -2.6f;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Follow();
    }

    void Follow()
    {
        // Player가 내려갈때는 카메라가 안따라가도록
        if (target.position.y < (transform.position.y - min_Y_Threshold))
        {
            followPlayer = false;
        }

        if (target.position.y > transform.position.y)
        {
            followPlayer = true;
        }

        if (followPlayer)
        {
            Vector3 temp = transform.position;
            temp.y = target.position.y;
            transform.position = temp;
        }
    }

}
