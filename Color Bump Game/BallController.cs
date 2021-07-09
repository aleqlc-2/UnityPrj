using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private float thrust = 250f;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float wallDistance = 5f;
    [SerializeField] private float minCamDistance = 3f;

    private Vector2 lastMousePos;

    private void FixedUpdate() // 고정프레임마다 호출. 물리효과 적용할 때 많이씀
    {
        if (GameManager.singleton.GameEnded)
            return;

        if (GameManager.singleton.GameStarted)
        {
            rb.MovePosition(transform.position + Vector3.forward * Time.fixedDeltaTime * 5);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.singleton.GameEnded)
            return;

        if (collision.gameObject.tag == "Death")
            GameManager.singleton.EndGame(false); // win변수에 false를 줌
    }

    void Update() // 불규칙하게 호출될 수 있음
    {
        Vector2 deltaPos = Vector2.zero;

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Input.mousePosition;

            if (lastMousePos == Vector2.zero)
                lastMousePos = currentMousePos;

            deltaPos = currentMousePos - lastMousePos;
            lastMousePos = currentMousePos;

            Vector3 force = new Vector3(deltaPos.x, 0, deltaPos.y) * thrust;
            rb.AddForce(force);
        }
        else // 마우스떼면 lastMousePos값 초기화
        {
            lastMousePos = Vector2.zero;
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;

        if (transform.position.x < -wallDistance)
        {
            pos.x = -wallDistance;
        }
        else if (transform.position.x > wallDistance)
        {
            pos.x = wallDistance;
        }

        // 볼이 카메라 뒤로 사라지지 않도록
        if (transform.position.z < Camera.main.transform.position.z + minCamDistance)
        {
            pos.z = Camera.main.transform.position.z + minCamDistance;
        }

        transform.position = pos;
    }
}