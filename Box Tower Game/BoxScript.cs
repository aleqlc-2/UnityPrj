using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    private float min_X = -2.2f, max_X = 2.2f;
    private float move_Speed = 2f;

    private bool canMove;
    private bool gameOver;
    private bool ignoreCollision;
    private bool ignoreTrigger;

    private Rigidbody2D myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        myBody.gravityScale = 0f; // 누르기전엔 떨어지지않고 위에서 부유하도록
    }

    void Start()
    {
        canMove = true;

        if (Random.Range(0, 2) > 0)
        {
            move_Speed *= -1f;
        }

        GameplayController.instance.currentBox = this;
    }

    void Update()
    {
        MoveBox();
    }

    void MoveBox()
    {
        if (canMove)
        {
            Vector3 temp = transform.position; // 속도와 방향을 결정하기위해 임시변수에 넣음
            temp.x += move_Speed * Time.deltaTime; // 속도결정

            // 방향결정
            if (temp.x > max_X)
            {
                move_Speed *= -1f;
            }
            else if (temp.x < min_X)
            {
                move_Speed *= -1f;
            }

            transform.position = temp; // 이동
        }
    }

    public void DropBox()
    {
        canMove = false;
        myBody.gravityScale = Random.Range(2, 4);
    }

    void Landed()
    {
        if (gameOver)
            return;

        ignoreCollision = true; // 박스를 1개씩만 생성하기 위해
        ignoreTrigger = true;

        GameplayController.instance.SpawnNewBox();
        GameplayController.instance.MoveCamera();
    }

    void RestartGame()
    {
        GameplayController.instance.RestartGame();
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        // 박스를 1개씩만 생성하기 위해
        if (ignoreCollision)
            return;

        if (target.gameObject.tag == "Platform")
        {
            Invoke("Landed", 1f); // 상자가 충돌하면 Landed가 1초후에 Invoke
        }

        if (target.gameObject.tag == "Box")
        {
            Invoke("Landed", 1f); // 상자가 충돌하면 Landed가 1초후에 Invoke
        }
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        // 이 코드 없으면 3개쌓아서 올라갈때
        // 젤 밑의 박스가 GameOver에 닿아서 게임이 재시작되어버림
        // 상자가 충돌하면 Landed가 1초후에 Invoke되어 ignoreTrigger가 true가 되므로
        // 1초가 지난뒤에 상자가 무너지면 게임이 재시작되지 않음
        if (ignoreTrigger)
             return;

        // 즉, 이 구문은 상자가 충돌한 후 1초 안에 GameOver에 닿아야 들어올 수 있는 구문임
        if (target.tag == "GameOver")
        {
            // 충돌한 후 1초후에 Landed가 Invoke되므로
            // 1초가 되기 전 이 구문에 들어왔으므로 취소가능
            CancelInvoke("Landed");
            
            gameOver = true; // ?
            ignoreTrigger = true; // ?

            Invoke("RestartGame", 1f);
        }
    }
}
