using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15f;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1f);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = travelDirection * speed; // 여기서 ball이 움직이는거임
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero) // ray가 벽을 맞췄다면
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1) // 벽에 부딪혔다면
            {
                // ball 멈춤
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        // 볼이 일단 움직이기시작하면 아래 input의 영향을 받지 않고 벽에 부딪힐때까지 가도록
        if (isTraveling)
            return;

        if (Input.GetMouseButton(0))
        {
            // 마우스 클릭한 현재 위치 저장
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero) // Vector2.zero는 게임화면 좌상단
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                // 일정량 이상 마우스를 움직여줘야 ball이 움직이도록
                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize(); // 방향화

                // Up and Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    // GO Up and Down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    // GO Left and right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }
            swipePosLastFrame = swipePosCurrentFrame; // 마우스 클릭한 이전 위치 저장
        }

        // 마우스 떼면 초기화
        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    } // FixedUpdate

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }
}
