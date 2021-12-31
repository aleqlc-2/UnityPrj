using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Animator anim;

    private CharacterController charController;

    // CharacterController.Move()가 벽,바닥,공중,천장과의 충돌
    private CollisionFlags collisionFlags = CollisionFlags.None;

    private float moveSpeed = 5f;
    private float player_ToPointDistance;
    private float gravity = 9.8f;
    private float height;

    private bool canMove;

    private bool finished_Movement = true;
    public bool FinishedMovement
    {
        get { return finished_Movement; }
        set { finished_Movement = value; }
    }

    private Vector3 target_Pos = Vector3.zero;
    public Vector3 TargetPosition
    {
        get { return target_Pos; }
        set { target_Pos = value; }
    }

    private Vector3 player_Move = Vector3.zero;

    void Awake()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        CalculateHeight();
        CheckIfFinishedMovement();
    }

    private void CalculateHeight()
    {
        if (IsGrounded()) height = 0f;
        else height -= gravity * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        return collisionFlags == CollisionFlags.CollidedBelow;
    }

    private void CheckIfFinishedMovement()
    {
        if (!finished_Movement)
        {
            // 현재 애니메이션에서 다른 상태로 전환하지 않았고 && 현재 진행되고 있는 애니메이션이 Stand가 아니고
            // && 현재 애니메이션이 80% 이상 진행되었다면
            if (!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand")
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                finished_Movement = true; // 즉, Attack과 같은 애니메이션을 실행하면 80% 이상 진행해야 move할 수 있음
            }
        }
        else
        {
            MoveThePlayer();
            player_Move.y = height * Time.deltaTime;
            collisionFlags = charController.Move(player_Move);
        }
    }

    private void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ray에 맞은 콜라이더가 TerrainCollider 자료형으로 캐스팅이 가능하면 true, 불가능하면 false
                if (hit.collider is TerrainCollider)
                {
                    player_ToPointDistance = Vector3.Distance(transform.position, hit.point);

                    if (player_ToPointDistance >= 1.0f)
                    {
                        canMove = true;
                        target_Pos = hit.point;
                    }
                }
            }
        }

        // 이 코드가 if (Input.GetMouseButtonDown(0)) 안에 있으면 마우스 클릭한 순간에만 동작하므로
        // 클릭한 쪽으로 완전히 바라보지 못하게됨.
        if (canMove)
        {
            anim.SetFloat("Walk", 1.0f);

            Vector3 target_Temp = new Vector3(target_Pos.x, transform.position.y, target_Pos.z);

            // 클릭한쪽으로 바라보기
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(target_Temp - transform.position),
                                                  15.0f * Time.deltaTime);

            player_Move = transform.forward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target_Pos) <= 0.1f) canMove = false;
        }
        else
        {
            player_Move.Set(0f, 0f, 0f);
            anim.SetFloat("Walk", 0f);
        }
    }
}
