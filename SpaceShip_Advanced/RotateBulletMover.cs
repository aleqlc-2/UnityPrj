using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBulletMover : MonoBehaviour
{
    private float speed = 15f;

    void Update()
    {
        Move();
    }

    private void Move()
	{
        Vector3 temp = transform.position;
        temp -= transform.up * Time.deltaTime * speed;
        transform.position = temp;
	}
}
