using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBack : MonoBehaviour
{
    public Transform head;
    
    private float speed = 35f;

    void OnCollisionEnter()
    {
        Rigidbody r = BallManager.Instance.CurrentBall.GetComponent<Rigidbody>();
        r.angularVelocity = Vector3.zero;

        // normalized 안하는게 맞을듯 지우고 실행해봐
        Vector3 direction = (head.position - r.transform.position).normalized;
        r.velocity = direction * speed;
    }
}
