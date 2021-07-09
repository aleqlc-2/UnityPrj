using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float speed = 5f;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 temp = transform.position;
        temp += transform.up * Time.deltaTime * speed;
        transform.position = temp;
    }
}
