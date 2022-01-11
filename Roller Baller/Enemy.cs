using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float minDistance = 1f;
    private float distance;
    [SerializeField] private float minMoveSpeed = 0.5f, maxMoveSpeed = 2f;
    private float moveSpeed;

    private Transform playerTarget;

    void Start()
    {
        SetMoveSpeed();
    }

    void Update()
    {
        if (!playerTarget) return;

        transform.LookAt(playerTarget); // 유니티상에서 x,y,z축은 그대로있고 transform의 회전값만 변하고 타겟을 바라보는 쪽이 forward이동임

        distance = Vector3.Distance(transform.position, playerTarget.position);

        // Enemy가 플레이어 추적
        if (distance > minDistance) transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void SetMoveSpeed()
    {
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
}
