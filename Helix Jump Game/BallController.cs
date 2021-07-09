using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool ignoreNextCollision;
    public Rigidbody rb;
    public float impulseForce = 5f;
    private Vector3 startPos;

    public int perfectPass = 0;
    public bool isSuperSpeedActive;
    
    void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreNextCollision)
            return;

        if (isSuperSpeedActive) // 슈퍼스피드 상태이면
        {
            if (!collision.transform.GetComponent<Goal>())
            {
                // Goal을 제외하고 부딪히는 조각의 parent(즉, 원판 전체)를 파괴
                // 슈퍼스피드 상태에서는 충돌해도 HitDeathPart(); 메서드를 호출하지않으므로
                // 슈퍼스피드 상태에서 deathPart 때려도 게임 재시작되지않고 원판 전체를 파괴
                Destroy(collision.transform.parent.gameObject, 0.3f);
                isSuperSpeedActive = false; // 슈퍼스피드 상태로 한번만 Destroy할 수 있도록
            }
        }
        else // 보통스피드 상태이면
        {
            // Ball이 충돌한 위치의 컴포넌트가 deathPart이면 HitDeathPart 메서드 실행(Scene 재시작)
            DeathPart deathPart = collision.transform.GetComponent<DeathPart>();
            if (deathPart)
                deathPart.HitDeathPart();
        }

        rb.velocity = Vector3.zero; // 이 코드 안쓰면 공이 불규칙적으로 점핑하다가 멈춤
        rb.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);

        // 밑에 두줄과 ignoreNextCollision관련코드 다 없어져도 동작 동일한듯.. 실행해볼것
        ignoreNextCollision = true; // 뛰어올랐을때는 충돌무시했다가
        Invoke("AllowCollision", 0.2f); // 0.2초후에 다시 충돌체크

        GameManager.singleton.AddScore(1);

        perfectPass = 0; // 어딘가에 부딪히는 순간 perfectPass를 0으로 초기화
    }

    private void Update()
    {
        // 3번 이상 어딘가에 부딪히지 않고 통과하면
        if (perfectPass >= 3 && !isSuperSpeedActive)
        {
            isSuperSpeedActive = true; // 슈퍼스피드 상태 활성화
            rb.AddForce(Vector3.down * 10, ForceMode.Impulse); // 속도증가
        }
    }

    private void AllowCollision()
    {
        ignoreNextCollision = false;
    }

    // 게임 재시작시 볼의 위치 초기화
    public void ResetBall()
    {
        transform.position = startPos;
    }
}
