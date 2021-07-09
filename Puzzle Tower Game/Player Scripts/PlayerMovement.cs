using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 targetForward;
    private Vector3 dPos;

    public float moveSpeed = 300f;
    private float smoothMovement = 1f;

    private bool canMove;

    private Camera mainCam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetForward = transform.forward;

        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        GetInput();
        UpdateForward();
    }

    void MovePlayer()
    {
        if (canMove)
        {
            // 델타 포지션
            dPos = new Vector3(Input.GetAxisRaw(Axis.MOUSE_X), 0f, Input.GetAxisRaw(Axis.MOUSE_Y)); // 위치를
            dPos.Normalize(); // 방향으로 바꿔서
            dPos *= moveSpeed * Time.fixedDeltaTime; // 스피드와 시간을 추가로 곱하여 연속적으로 움직이도록

            // 왜 *= 이 안되지, dPoS를 앞에 곱하는것도 안됨
            // 움직이는 방향을 바라보도록 회전에 적용
            // 쿼터니언 * Vector3 하면 Vector3가 되는듯.
            // 거꾸로 곱하면 쿼터니언이 되진 않음. 거꾸로 곱하는 거 자체가 안됨..
            dPos = Quaternion.Euler(0f, mainCam.transform.eulerAngles.y, 0f) * dPos;

            rb.MovePosition(rb.position + dPos); // 움직임 실행, 아직 회전이 적용되진 않음

            // 아래 코드 없으면 플레이어가 회전 적용이 안되고 움직임.
            if (dPos != Vector3.zero)
            {
                // 인자에 dPos 넣으면 플레이어가 거꾸로 걸음, 블루축이 플레이어의 뒤를 향하고 있어서 그런듯.
                targetForward = Vector3.ProjectOnPlane(-dPos, Vector3.up);
            }
        }
    }

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canMove = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            canMove = false;
        }
    }

    void UpdateForward()
    {
        transform.forward = Vector3.Slerp(
            transform.forward, targetForward, Time.deltaTime * smoothMovement);
    }
}
