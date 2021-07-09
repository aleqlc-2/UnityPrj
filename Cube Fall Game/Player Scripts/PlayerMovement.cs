using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D myBody;

    public float moveSpeed = 2f;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    // 웬만하면 0이 아니라 myBody.velocity.y로 넣어줘야하는게
    // 0을 넣으면 자기자신의 속도가 아니라 중력으로만 이동하기때문에 떨어지는 속도가 느려지고 불규칙적이됨.
    void Move()
    {
        if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);
        }

        if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            myBody.velocity = new Vector2(-moveSpeed, myBody.velocity.y);
        }
    }

    public void PlatformMove(float x)
    {
        myBody.velocity = new Vector2(x, myBody.velocity.y);
    }

}