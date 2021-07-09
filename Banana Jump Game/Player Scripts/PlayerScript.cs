using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D myBody;

    public float move_Speed = 2f;

    public float normal_Push = 10f;
    public float extra_Push = 14f;

    private bool initial_Push; // 기본값은 false

    private int push_Count;

    private bool player_Died;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (player_Died)
            return;

        // myBody.velocity.y 대신 0을 적어버리면 push당해서 올라가는중에 키를 누르는 순간 
        // y가 0이 되서 올라가던게 멈춰버리고 좌우로만 움직이면서 중력적용으로 아래로 떨어짐
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            myBody.velocity = new Vector2(move_Speed, myBody.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            myBody.velocity = new Vector2(-move_Speed, myBody.velocity.y);
        }
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (player_Died)
            return;

        if (target.tag == "ExtraPush") // 게임시작시 첫 Push
        {
            if (!initial_Push)
            {
                initial_Push = true;

                // 여기서부터는 myBody.velocity.x 대신 0 적어도 상관없음
                // 왜냐하면 키를 누르면 위의 코드에서 x가 적용될 것이기 때문
                myBody.velocity = new Vector2(myBody.velocity.x, 18f);
                target.gameObject.SetActive(false);

                SoundManager.instance.JumpSoundFX();

                return;
            }
        }

        if (target.tag == "NormalPush") // 첫 Push 이후 Push
        {
            myBody.velocity = new Vector2(myBody.velocity.x, normal_Push);

            target.gameObject.SetActive(false);

            push_Count++;

            SoundManager.instance.JumpSoundFX();
        }

        if (target.tag == "ExtraPush") // 첫 Push 이후 Push
        {
            myBody.velocity = new Vector2(myBody.velocity.x, extra_Push);

            target.gameObject.SetActive(false);

            push_Count++;

            SoundManager.instance.JumpSoundFX();
        }

        if (push_Count == 2) // 2번 Push하고 플랫폼 재생성
        {
            push_Count = 0;
            PlatformSpawner.instance.SpawnPlatforms();
        }

        if (target.tag == "FallDown" || target.tag == "Bird")
        {
            player_Died = true;

            SoundManager.instance.GameOverSoundFX();
            
            GameManager.instance.RestartGame();
        }
    }
}
