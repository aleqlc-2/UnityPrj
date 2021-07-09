using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody r;

    private bool isHit = false;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    // 이 코드를 없애면 되지 않을까?
    void FixedUpdate()
    {
        if (isHit & r.velocity.magnitude < 0.1f)
            Destroy(this.gameObject);
    }

    void OnCollisionEnter()
    {
        isHit = true;
    }
}
