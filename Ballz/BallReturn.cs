using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReturn : MonoBehaviour
{
    private BallLauncher ballLauncher;

    private void Awake()
    {
        ballLauncher = FindObjectOfType<BallLauncher>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ballLauncher.ReturnBall();

        // 리턴된 볼은 다시 튀겨나가지 않고 대기하도록 비활성화했다가
        // BallLauncher 클래스의 LaunchBalls()가 실행될때 다시 활성화
        collision.collider.gameObject.SetActive(false);
        // collider는 안써도됨.
        // 하지만 벽이든 ball이든 둘 다 콜라이더 컴포넌트는 있어야 OnCollisionEnter2D가 충돌을 인식함
    }
}
