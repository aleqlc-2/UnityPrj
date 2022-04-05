using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 movementDirection;

    private Rigidbody myBody;

    public float walk_Speed = 5f;
    public float walking_Force = 50f;
    public float turning_Smoothing = 0.1f;

    void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HangleMovement();
    }

    private void HangleMovement()
    {
        Vector3 targetVelocity = movementDirection * walk_Speed; // 도달하고자하는 타겟velocity
        Vector3 deltaVelocity = targetVelocity - myBody.velocity; // 타겟velocity - 현재velocity

        if (myBody.useGravity) deltaVelocity.y = 0f; // 중력적용된다면 바닥을 걸어야하므로 y값을 0으로

        myBody.AddForce(deltaVelocity * walking_Force, ForceMode.Acceleration); // ForceMode.Acceleration는 제로백현상 적용

        // 이동하는쪽으로 바라보도록하는 로직
        Vector3 face_Direction = movementDirection;
        if (face_Direction == Vector3.zero)
            myBody.angularVelocity = Vector3.zero; // 캐릭터 회전 멈춤
        else
        {
            float rotationAngle = AngleAroundAxis(transform.forward, face_Direction, Vector3.up);

            myBody.angularVelocity = Vector3.up * rotationAngle * turning_Smoothing;
        }
    }


    private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        float angle = Vector3.Angle(dirA, dirB); // 두 벡터사이의 각도(내각), 90도 이내의 각만 구해짐

        // Vector3.Dot는 두 벡터사이의 내적. 이 값을 Mathf.Acos의 인자에 넣으면 두 벡터사이의 각도(내각)이 구해짐
        // Vector3.Cross는 두 벡터사이의 외적. 인자의 두 벡터가 직교를 이룰때에 다른 직교방향값
        // 캐릭터가 최소회전거리로 회전하도록 하는 로직인듯
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) > 0 ? 1 : -1);
    }
}
