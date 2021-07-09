using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController character_Controller;

    private Vector3 move_Direction;

    public float speed = 5f;
    private float gravity = 20f;

    public float jump_Force = 10f;
    private float vertical_Velocity;

    void Awake()
    {
        character_Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        MoveThePlayer();
    }

    void MoveThePlayer()
    {
        // y축을 따라 이동하진 않도록
        move_Direction = new Vector3(Input.GetAxis(Axis.HORIZONTAL), 0f, Input.GetAxis(Axis.VERTICAL));

        // local space를 world space로 바꿈
        move_Direction = transform.TransformDirection(move_Direction);

        // 스피드와 시간 적용
        move_Direction *= speed * Time.deltaTime;

        ApplyGravity(); // 점프와 중력 적용

        // 이동
        character_Controller.Move(move_Direction);
    }

    void ApplyGravity()
    {
        // 굳이 if, else 구문 안써도 동작은 같음
        if (character_Controller.isGrounded) // 땅에 닿으면 계속 중력 적용
        {
            vertical_Velocity -= gravity * Time.deltaTime;
            PlayerJump();
        }
        else // 시작하면 공중에 떠있던 플레이어가 땅으로 내려옴
        {
            vertical_Velocity -= gravity * Time.deltaTime;
        }
        
        // Time.deltaTime 안곱하면 엄청 높이 순식간에 뛰어올랐다가 순식간에 내려오게됨.
        move_Direction.y = vertical_Velocity * Time.deltaTime;
    }

    void PlayerJump()
    {
        if (character_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            vertical_Velocity = jump_Force;
        }
    }
}
