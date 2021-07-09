using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayer : MonoBehaviour
{
    [SerializeField]
    PlayerManager playerManager;

    private Rigidbody rb;

    [SerializeField]
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 인스펙터에서 이미 할당하였으니 필요없는코드
        GetComponent<Renderer>().material = playerManager.collectedObjMat;

        playerManager.collidedList.Add(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Grounded();
        }
    }

    void Grounded()
    {
        isGrounded = true;
        playerManager.playerState = PlayerManager.PlayerState.Move;
        rb.useGravity = false;

        // 이 코드 없으면 player가 충돌때문에 이상한 위치와 회전을 함
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Destroy(this, 1);
    }
}
