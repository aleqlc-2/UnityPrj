using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float jumpForce;
    private float speed = 150f;
    private float horizontal;
    private float vertical;

    private SpriteRenderer renderer;
    private Rigidbody2D myBody;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Spikes") gameObject.SetActive(false);

        if (collision.tag == "Pickables") collision.gameObject.SetActive(false);
    }

    void Update()
    {
        // Input.GetAxis("Horizontal"); 안누르면 0, 왼쪽방향키누르면 -1까지 점점커짐, 오른쪽방향키누르면 1까지 점점커짐
        // Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime; 안누르면 0, 왼쪽방향키누르면 -0.04까지 점점커짐, 오른쪽방향키누르면 0.04까지 점점커짐
        // Input.GetAxis("Vertical"); 안누르면 0, 아래쪽방향키누르면 -1까지 점점커짐, 위쪽방향키누르면 1까지 점점커짐
        horizontal = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        vertical = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        // 부동 소수점(float)은 부정확하기에 비교를 위해서 == 연산자보다는 Mathf.Approximately로 비교해야함
        // 즉, 1.0 == 10.0 / 10.0 은 true를 반환하지 않을수도 있음
        // flipX는 체크하면 현재 스프라이트를 수평으로 반대로 돌림
        // 즉, 게임시작시 스프라이트가 오른쪽을 보고 있으므로 왼쪽방향키를 누르면 왼쪽을 바라보도록
        if (!Mathf.Approximately(horizontal, 0)) renderer.flipX = (horizontal < 0);

        Vector3 targetVelocity = new Vector2(horizontal, myBody.velocity.y);
        Vector3 refVelocity = Vector3.zero;
        myBody.velocity = Vector3.SmoothDamp(myBody.velocity, targetVelocity, ref refVelocity, 0.03f);

        if (!Mathf.Approximately(vertical, 0)) // 위방향키 또는 아랫방향키 눌렀을때
        {
            if (Mathf.Approximately(myBody.velocity.y, 0)) // 점프 또는 하강중이 아니면
            {
                // XOR연산자(두 값이 같으면 0, 다르면 1)
                // 즉, 아래쪽방향키 눌렀고 myBody.gravityScale가 음수거나 위쪽방향키 눌렀고 myBody.gravityScale가 양수이면
                if (vertical < 0 ^ myBody.gravityScale > 0)
                {
                    myBody.gravityScale *= -1; // 중력 토글
                    renderer.flipY = (vertical > 0); // 스프라이트 상하 토글
                }
            }
        }

        if (Mathf.Approximately(myBody.velocity.y, 0)) // 위아래로 움직이지 않는 상태에서
        {
            if (Input.GetKey(KeyCode.Space)) // 스페이스 누르면
            {
                // gravityScale값에 따라 방향 설정하여 AddForce
                Vector3 direction = (myBody.gravityScale > 0) ? Vector3.up : Vector3.down;
                myBody.AddForce(jumpForce * direction, ForceMode2D.Impulse);
            }
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}
