using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardAndBackwardScript : MonoBehaviour
{
    public float moveSpeed = 10f;

    public bool moveForwardAndBackward;

    void Start()
    {
        if (moveForwardAndBackward)
        {
            StartCoroutine(ChangeDirection());
        }
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    // 3ÃÊ ÈÄ¿¡ ºÎ¸Þ¶û µÇµ¹¾Æ¿È
    private IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(3f);
        moveSpeed *= -1f;
    }
}
