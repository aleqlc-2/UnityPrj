using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 200f;

    private Transform playerTarget;

    private Rigidbody2D myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        playerTarget = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (playerTarget == null) return;

        // (Vector2)를 캐스팅하여 z는 날림
        Vector2 point2Target = (Vector2)transform.position - (Vector2)playerTarget.position;
        point2Target.Normalize();

        float value = Vector3.Cross(point2Target, transform.up).z;

        myBody.velocity = transform.up * speed; // Time.deltaTime곱하면 엄청느리게 감. 속도 그 자체이기 때문에 그대로 할당
        myBody.angularVelocity = value * rotationSpeed;
    }
}
