using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float movementForce = 0.5f, jumpForce = 0.15f;
    private float jumpTime = 0.15f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Jump(true);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Jump(false);
        }
    }

    void Jump(bool left)
    {
        SoundManager.instance.JumpSound();
        
        // using
        if (left)
        {
            transform.DORotate(new Vector3(0f, 90f, 0f), 0f); // 회전인데 Vector3를 주네..

            rb.DOJump(new Vector3(transform.position.x - movementForce,
                                  transform.position.y + jumpForce,
                                  transform.position.z),
                                  0.5f, 1, jumpTime);
        }
        else
        {
            transform.DORotate(new Vector3(0f, -180f, 0f), 0f); // 회전인데 Vector3를 주네..

            rb.DOJump(new Vector3(transform.position.x,
                                  transform.position.y + jumpForce,
                                  transform.position.z + movementForce),
                                  0.5f, 1, jumpTime);
        }
    }
}
