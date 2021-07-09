using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // new한정자로 액세스 가능한 멤버를 숨김
    // new 안써도 동작차이가 없음..
    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private float moveSpeed = 10f;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // BallLauncher에서 AddForce를 하고 여기서도 velocity로 쏘네..
    // 둘 중 하나라도 없으면 발사안됨..
    private void Update()
    {
        rigidbody2D.velocity = rigidbody2D.velocity.normalized * moveSpeed; // 방향 * 스피드
    }
}
