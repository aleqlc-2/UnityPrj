using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    float movement = 0f;
    public float movementSpeed = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Time.fixedDeltaTime 곱했더니 엄청 느리게 움직임
        // velocity에 할땐 안곱하고 rb.MovePosition할땐 곱하는듯.
        movement = Input.GetAxis("Horizontal") * movementSpeed; // GetAxisRaw가 더 깔끔하게 움직임
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = movement;
        rb.velocity = velocity;
    }
}
