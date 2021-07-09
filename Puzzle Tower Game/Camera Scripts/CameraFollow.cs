using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSmoothing = 10f;
    public float rotationSmoothing = 15f;

    private Transform target;

    private Vector3 targetForward;

    void Awake()
    {
        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;
    }

    void Start()
    {
        // // 아래 코드 두줄 없으면 카메라가 위에서 수직으로 찍음
        // targetForward = transform.forward;
        // targetForward.y = 0f;

        //Snap(); // 없어도 카메라 잘따라오는데..
    }

    void Update()
    {
        FollowPlayer();
    }

    // // 카메라의 위치가 다른곳에 있어도 target(플레이어)이 존재하면 카메라가 그 쪽으로 이동
    // void Snap()
    // {
    //     if (target != null)
    //     {
    //         transform.position = target.position;
    //     }

    //     Vector3 forward = targetForward;
    //     forward.y = transform.forward.y;
    //     transform.forward = forward;
    // }

    void FollowPlayer()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(
                transform.position, target.position, Time.deltaTime * moveSmoothing);
        }

        // // 아래 코드 없어도 똑같은데..
        // Vector3 forward = transform.forward;
        // forward.y = 0f;
        // forward = Vector3.Slerp(forward, targetForward, Time.deltaTime * rotationSmoothing);
        // forward.y = transform.forward.y;
        // transform.forward = forward;
    }
}
