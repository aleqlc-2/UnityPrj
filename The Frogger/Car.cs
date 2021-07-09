using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private Rigidbody2D myBody;
    public float speed = 1f;
    public float minSpeed = 8f;
    public float maxSpeed = 12f;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        // transform.right는 x축을 따라 오브젝트를 이동시킴.
        // trasnform.up은 y축을 따라 오브젝트를 이동시킴.
        // transform.right.y 쓰면 안움직임. 값이 0인듯
        Vector2 xMove = new Vector2(transform.right.x, 0); // x축이 가리키는 방향으로

        // Time.time쓰면 엄청빨리지나감
        myBody.MovePosition(myBody.position + xMove * Time.fixedDeltaTime * speed);
    }
}
