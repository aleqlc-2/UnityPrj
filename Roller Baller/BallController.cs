using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private BallPlayer ball;

    private Vector3 moveDirection;

    private bool jump;

    private float moveHorizontal, moveVertical;

    [SerializeField] private GameObject explosionParticle;

    void Awake()
    {
        ball = GetComponent<BallPlayer>();
    }

    void FixedUpdate()
    {
        ball.Move(moveDirection); // AddForce나 AddTorque는 이동할때 공의 자체회전도 이미 구현되어있나?
        ball.Jump(jump);
    }

    void Update()
    {
        moveHorizontal = Input.GetAxis(TagManager.HORIZONTAL_AXIS);
        moveVertical = Input.GetAxis(TagManager.VERTICAL_AXIS);

        jump = Input.GetKeyDown(KeyCode.Space);

        moveDirection = new Vector3(-moveVertical, 0f, moveHorizontal).normalized;
    }

    public void DestroyPlayer()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
